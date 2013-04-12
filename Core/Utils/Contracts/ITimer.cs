using System;

namespace Core.Utils.Contracts
{
    public interface ITimer
    {
        float TimeModifier { get; set; }

        TimeSpan ElapsedTime { get; }
        TimeSpan ActualElapsedTime { get; }
        TimeSpan TimeLostToPause { get; }

        bool IsPaused { get; }

        void Reset();
        void Pause();
        void Resume();
        void Update();
    }
}
