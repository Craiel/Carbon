namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Threading;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;

    public class SceneManager : ThreadableEngineComponent, ISceneManager
    {
        private readonly IDictionary<int, IScene> registeredScenes;
        private readonly IList<int> preparedScenes;
        private readonly IList<int> activeOverlays;

        private ICarbonGraphics currentGraphics;

        private int? activeScene;
        private int? suspendedScene;

        private TimeSpan lastUpdateTime;

        private TypedVector2<int> currentSize;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneManager()
        {
            this.registeredScenes = new Dictionary<int, IScene>();
            this.preparedScenes = new List<int>();
            this.activeOverlays = new List<int>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IScene ActiveScene
        {
            get
            {
                if (this.activeScene == null)
                {
                    return null;
                }

                return this.registeredScenes[(int)this.activeScene];
            }
        }

        public IScene SuspendedScene
        {
            get
            {
                if (this.suspendedScene == null)
                {
                    return null;
                }

                return this.registeredScenes[(int)this.suspendedScene];
            }
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            this.currentGraphics = graphics;

            base.Initialize(graphics);
        }

        public override void Unload()
        {
            foreach (int key in this.preparedScenes)
            {
                this.registeredScenes[key].Unload();
            }

            this.preparedScenes.Clear();
        }
        
        public bool IsPrepared(int key)
        {
            if (!this.registeredScenes.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            return this.preparedScenes.Contains(key);
        }

        public void Register(int key, IScene scene)
        {
            this.registeredScenes.Add(key, scene);
        }

        public void Activate(int key, bool suspendActive = false)
        {
            IThreadQueueOperationPayload payload = new ThreadQueuePayload { Data = Tuple.Create(key, suspendActive) };
            this.QueueOperation(x => this.ActivateScene(payload), this.lastUpdateTime);
        }

        public void ActivateOverlay(int key)
        {
            IThreadQueueOperationPayload payload = new ThreadQueuePayload { Data = key };
            this.QueueOperation(x => this.ActivateSceneOverlay(payload), this.lastUpdateTime);
        }

        public void Deactivate()
        {
            if (this.activeScene == null)
            {
                throw new InvalidOperationException("No scene active");
            }

            this.QueueOperation(this.DeactivateScene, this.lastUpdateTime);
        }

        public void DeactivateOverlay(int key)
        {
            IThreadQueueOperationPayload payload = new ThreadQueuePayload { Data = key };
            this.QueueOperation(x => this.DeactivateSceneOverlay(payload), this.lastUpdateTime);
        }

        public void Prepare(int key)
        {
            if (!this.registeredScenes.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            if (this.preparedScenes.Contains(key))
            {
                throw new InvalidOperationException("Scene was already prepared: " + key);
            }

            this.registeredScenes[key].Initialize(this.currentGraphics);
            this.registeredScenes[key].Resize(this.currentSize);
            this.preparedScenes.Add(key);
        }

        public void Reload(int? key = null)
        {
            IThreadQueueOperationPayload payload = new ThreadQueuePayload { Data = Tuple.Create(key) };
            this.QueueOperation(x => this.ReloadScene(payload), this.lastUpdateTime);
        }

        public override bool Update(ITimer gameTime)
        {
            this.lastUpdateTime = gameTime.ElapsedTime;

            this.ProcessOperations(this.lastUpdateTime);

            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.suspendedScene != null)
            {
                this.registeredScenes[(int)this.suspendedScene].Update(gameTime);
            }

            if (this.activeScene != null)
            {
                this.registeredScenes[(int)this.activeScene].Update(gameTime);
            }

            IList<int> overlays = new List<int>(this.activeOverlays);
            foreach (int key in overlays)
            {
                this.registeredScenes[key].Update(gameTime);
            }

            return true;
        }

        public void Render(IFrameManager frameManager)
        {
            if (this.suspendedScene != null && this.registeredScenes[(int)this.suspendedScene].IsVisible)
            {
                // Todo: We probably need to act here to get this rendered as a inactive backend
                this.registeredScenes[(int)this.suspendedScene].Render(frameManager);
            }

            if (this.activeScene != null)
            {
                this.registeredScenes[(int)this.activeScene].Render(frameManager);
            }

            IList<int> overlays = new List<int>(this.activeOverlays);
            foreach (int key in overlays)
            {
                this.registeredScenes[key].Render(frameManager);
            }
        }

        public void Resize(TypedVector2<int> size)
        {
            this.currentSize = size;
            foreach (int key in this.preparedScenes)
            {
                this.registeredScenes[key].Resize(size);
            }
        }

        private bool ReloadScene(IThreadQueueOperationPayload payload)
        {
            int? key = ((Tuple<int?>)payload.Data).Item1;

            if (key != null && !this.registeredScenes.ContainsKey((int)key))
            {
                throw new ArgumentException();
            }

            if (key == null)
            {
                foreach (int preparedKey in this.preparedScenes)
                {
                    this.registeredScenes[preparedKey].Unload();
                    this.registeredScenes[preparedKey].Initialize(this.currentGraphics);
                    this.registeredScenes[preparedKey].Resize(this.currentSize);
                }
            }
            else
            {
                this.registeredScenes[(int)key].Unload();
                this.registeredScenes[(int)key].Initialize(this.currentGraphics);
                this.registeredScenes[(int)key].Resize(this.currentSize);
            }

            return true;
        }

        private bool ActivateScene(IThreadQueueOperationPayload payload)
        {
            int key = ((Tuple<int, bool>)payload.Data).Item1;
            bool suspendActive = ((Tuple<int, bool>)payload.Data).Item2;

            if (!this.registeredScenes.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            if (this.activeScene == key)
            {
                throw new InvalidOperationException("Scene is already active: " + key);
            }

            if (this.activeOverlays.Contains(key))
            {
                throw new ArgumentException("Can not activate scene while it is set as overlay: " + key);
            }

            if (this.activeScene != null)
            {
                IScene lastActive = this.registeredScenes[(int)this.activeScene];
                lastActive.IsActive = false;
                if (suspendActive)
                {
                    this.suspendedScene = this.activeScene;
                }
                else
                {
                    lastActive.IsVisible = false;
                    lastActive.Unload();
                    this.preparedScenes.Remove((int)this.activeScene);
                }

                this.activeScene = null;
            }

            IScene scene = this.registeredScenes[key];
            if (!this.preparedScenes.Contains(key))
            {
                scene.Initialize(this.currentGraphics);
                this.preparedScenes.Add(key);
            }

            // Call checkstate to see if everything is proper for activation
            scene.CheckState();

            scene.IsActive = true;
            scene.IsVisible = true;
            this.activeScene = key;

            return true;
        }

        private bool DeactivateScene(IThreadQueueOperationPayload arg)
        {
            if (this.activeScene == null)
            {
                throw new InvalidOperationException();
            }

            var key = (int)this.activeScene;
            IScene active = this.registeredScenes[key];
            active.Unload();
            this.preparedScenes.Remove(key);
            active.IsActive = false;
            active.IsVisible = false;
            this.activeScene = null;

            if (this.suspendedScene != null)
            {
                this.activeScene = this.suspendedScene;
                active = this.registeredScenes[key];
                active.IsActive = true;
                active.IsVisible = true;
            }

            return true;
        }

        private bool ActivateSceneOverlay(IThreadQueueOperationPayload payload)
        {
            var key = (int)payload.Data;
            if (!this.registeredScenes.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            if (this.activeScene == key)
            {
                throw new ArgumentException("Can not set the active scene as overlay: " + key);
            }

            if (this.activeOverlays.Contains(key))
            {
                throw new InvalidOperationException("Scene overlay is already active: " + key);
            }

            IScene scene = this.registeredScenes[key];
            if (!this.preparedScenes.Contains(key))
            {
                scene.Initialize(this.currentGraphics);
                this.preparedScenes.Add(key);
            }

            // Call checkstate to see if everything is proper for activation
            scene.CheckState();

            scene.IsActive = true;
            scene.IsVisible = true;

            this.activeOverlays.Add(key);

            return true;
        }

        private bool DeactivateSceneOverlay(IThreadQueueOperationPayload payload)
        {
            var key = (int)payload.Data;
            if (!this.registeredScenes.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            if (!this.activeOverlays.Contains(key))
            {
                throw new InvalidOperationException("Scene overlay is not active: " + key);
            }

            IScene active = this.registeredScenes[key];
            active.Unload();
            this.preparedScenes.Remove(key);
            active.IsActive = false;
            active.IsVisible = false;

            this.activeOverlays.Remove(key);
            return true;
        }
    }
}
