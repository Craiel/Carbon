namespace GrandSeal.DataDemon.Logic
{
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

        public virtual void Dispose()
        {
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
        protected void SetProgress(int max, int current)
        {
            this.ProgressMax = max;
            this.Progress = current;
        }
    }
}
