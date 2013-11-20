namespace GrandSeal.DataDemon.Contracts
{
    using System;

    public interface IDemonLogic : IDisposable
    {
        TimeSpan RefreshInterval { get; }

        bool LoadConfig(string config);
        
        void Refresh();
    }
}
