using System;
using System.Collections.Generic;

namespace Carbon.Engine.Rendering
{
    using Carbon.Engine.Contracts.Rendering;

    public class RenderableList<T> : IRenderable
        where T : class, IRenderable
    {
        private readonly IList<WeakReference<T>> entries;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RenderableList()
        {
            this.entries = new List<WeakReference<T>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Add(T entry)
        {
            this.entries.Add(new WeakReference<T>(entry));
        }

        public void Clear()
        {
            this.entries.Clear();
        }

        public void Render(FrameInstructionSet activeSet)
        {
            if (this.entries.Count <= 0)
            {
                return;
            }

            lock (this.entries)
            {
                foreach (WeakReference<T> reference in this.entries)
                {
                    T entry;
                    if (reference.TryGetTarget(out entry))
                    {
                        entry.Render(activeSet);
                    }
                }
            }
        }
    }
}
