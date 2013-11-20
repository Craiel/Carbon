namespace GrandSeal.DataDemon.Contracts
{
    using System;

    public interface IDemonOperation : IDisposable
    {
        string Name { get; }

        bool IsSuspended { get; }

        int EntriesToProcess { get; }

        int Progress { get; }
        int ProgressMax { get; }

        void Refresh();
        void Process();

        void Suspend();
        void Resume();
    }
}
