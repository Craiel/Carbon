using System.Collections.Generic;

namespace Carbon.Engine.Scene
{
    using Carbon.Engine.Contracts.Scene;

    public class SceneEntityCollection<T> : SceneEntity
        where T : class, ISceneEntity
    {
        private readonly List<T> list;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneEntityCollection()
        {
            this.list = new List<T>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyCollection<T> Entries
        {
            get
            {
                return this.list.AsReadOnly();
            }
        }

        public void Add(T entry)
        {
            this.list.Add(entry);
        }

        public void Remove(T entry)
        {
            this.list.Remove(entry);
        }

        public override bool Update(Core.Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            lock (this.list)
            {
                foreach (T entry in this.list)
                {
                    if (!entry.Update(gameTime))
                    {
                        return false;
                    }

                    // Update our child collection's world matrix by our the collections local matrix
                    entry.World = this.Local * entry.Local;
                }
            }

            return true;
        }

        public override void Render(Rendering.FrameInstructionSet frameSet)
        {
            base.Render(frameSet);

            lock (this.list)
            {
                foreach (T entry in this.list)
                {
                    entry.Render(frameSet);
                }
            }
        }
    }
}
