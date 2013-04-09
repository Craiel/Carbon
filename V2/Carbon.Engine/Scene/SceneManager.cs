using System;
using System.Collections.Generic;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Logic;

namespace Carbon.Engine.Scene
{
    public class SceneManager : EngineComponent, ISceneManager
    {
        private readonly IList<IScene> registeredScenes;
        private readonly IList<IScene> preparedScenes;

        private ICarbonGraphics currentGraphics;

        private IScene activeScene;
        private IScene suspendedScene;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneManager()
        {
            this.registeredScenes = new List<IScene>();
            this.preparedScenes = new List<IScene>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IScene ActiveScene
        {
            get
            {
                return this.activeScene;
            }
        }

        public IScene SuspendedScene
        {
            get
            {
                return this.suspendedScene;
            }
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            this.currentGraphics = graphics;

            base.Initialize(graphics);
        }

        public override void Unload()
        {
            foreach (IScene scene in this.preparedScenes)
            {
                scene.Unload();
            }

            this.preparedScenes.Clear();
        }
        
        public bool IsPrepared(IScene scene)
        {
            if (scene == null || !this.registeredScenes.Contains(scene))
            {
                throw new ArgumentException();
            }

            return this.preparedScenes.Contains(scene);
        }

        public void Register(IScene scene)
        {
            this.registeredScenes.Add(scene);
        }

        public void Activate(IScene scene, bool suspendActive = false)
        {
            if (scene == null || !this.registeredScenes.Contains(scene))
            {
                throw new ArgumentException();
            }

            if (this.activeScene == scene)
            {
                throw new InvalidOperationException("Scene is already active: " + scene);
            }

            if (this.activeScene != null)
            {
                this.activeScene.IsActive = false;
                if (suspendActive)
                {
                    this.suspendedScene = this.activeScene;
                }
                else
                {
                    this.activeScene.IsVisible = false;
                    this.activeScene.Unload();
                }

                this.activeScene = null;
            }

            if (!this.preparedScenes.Contains(scene))
            {
                scene.Initialize(this.currentGraphics);
                this.preparedScenes.Add(scene);
            }

            scene.IsActive = true;
            scene.IsVisible = true;
            this.activeScene = scene;
        }

        public void Deactivate()
        {
            if (this.activeScene == null)
            {
                throw new InvalidOperationException("No active scene to deactivate");
            }

            this.activeScene.Unload();
            this.preparedScenes.Remove(this.activeScene);
            this.activeScene.IsActive = false;
            this.activeScene.IsVisible = false;
            this.activeScene = null;

            if (this.suspendedScene != null)
            {
                this.activeScene = this.suspendedScene;
                this.activeScene.IsActive = true;
                this.activeScene.IsVisible = true;
            }
        }

        public void Prepare(IScene scene)
        {
            if (scene == null || !this.registeredScenes.Contains(scene))
            {
                throw new ArgumentException();
            }

            if (this.preparedScenes.Contains(scene))
            {
                throw new InvalidOperationException("Scene was already prepared: " + scene);
            }

            scene.Initialize(this.currentGraphics);
            this.preparedScenes.Add(scene);
        }

        public void Reload(IScene scene = null)
        {
            if (scene != null && !this.registeredScenes.Contains(scene))
            {
                throw new ArgumentException();
            }

            if (scene == null)
            {
                foreach (IScene preparedScene in this.preparedScenes)
                {
                    preparedScene.Unload();
                    preparedScene.Initialize(this.currentGraphics);
                }
            }
            else
            {
                scene.Unload();
                scene.Initialize(this.currentGraphics);
            }
        }
    }
}
