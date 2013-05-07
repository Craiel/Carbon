using System.Collections.Generic;

using Carbon.Engine.Contracts.Logic;

using Core.Utils.Contracts;

namespace Carbon.Engine.Logic
{
    public class EngineComponentUpdateList<T>
        where T : IEngineComponent
    {
        private readonly IList<T> list;
        private readonly Stack<T> updateStack;

        public EngineComponentUpdateList()
        {
            this.list = new List<T>();
            this.updateStack = new Stack<T>();
        }

        public void Invalidate(T entry)
        {
            this.list.Add(entry);
            this.updateStack.Push(entry);
        }

        public bool Update(ITimer gameTime)
        {
            if (this.updateStack.Count <= 0)
            {
                return true;
            }

            lock (this.updateStack)
            {
                T entry;
                while ((entry = this.updateStack.Pop()) != null)
                {
                    entry.Update(gameTime);
                }
            }
        }
    }
}
