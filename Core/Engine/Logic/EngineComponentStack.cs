using System.Collections.Generic;

using Core.Engine.Contracts.Logic;

using Core.Utils.Contracts;
using System;

namespace Core.Engine.Logic
{
    public class EngineComponentStack<T> : EngineComponent
        where T : class, IEngineComponent
    {
        private readonly Stack<WeakReference<T>> initializeStack; 
        private readonly Stack<WeakReference<T>> updateStack;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EngineComponentStack()
        {
            this.initializeStack = new Stack<WeakReference<T>>();
            this.updateStack = new Stack<WeakReference<T>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void PushInitialize(T entry)
        {
            this.initializeStack.Push(new WeakReference<T>(entry));
        }

        public void PushUpdate(T entry)
        {
            this.updateStack.Push(new WeakReference<T>(entry));
        }

        public void Clear()
        {
            this.initializeStack.Clear();
            this.updateStack.Clear();
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            if (this.initializeStack.Count <= 0)
            {
                return;
            }

            lock (this.initializeStack)
            {
                while (this.initializeStack.Count > 0)
                {
                    T target;
                    if (this.initializeStack.Pop().TryGetTarget(out target))
                    {
                        target.Initialize(graphics);
                    }
                }
            }
        }

        public override bool Update(ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.updateStack.Count <= 0)
            {
                return true;
            }

            lock (this.updateStack)
            {
                while (this.updateStack.Count > 0)
                {
                    T target;
                    if (this.updateStack.Pop().TryGetTarget(out target))
                    {
                        target.Update(gameTime);
                    }
                }
            }

            return true;
        }
    }
}
