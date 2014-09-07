namespace GrandSeal.DataDemon.Logic
{
    using System;

    using GrandSeal.DataDemon.Contracts;

    public abstract class DemonOperation : IDemonOperation
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Name { get; }

        public bool IsSuspended { get; private set; }

        public int EntriesToProcess { get; protected set; }

        public int Progress { get; private set; }

        public int ProgressMax { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Refresh();

        public abstract void Process();

        public virtual void Suspend()
        {
            this.IsSuspended = true;
        }

        public virtual void Resume()
        {
            this.IsSuspended = false;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
        }

        protected void SetProgress(int max, int current)
        {
            this.ProgressMax = max;
            this.Progress = current;
        }
    }
}
