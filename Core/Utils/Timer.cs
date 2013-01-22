using System;
using System.Diagnostics;
using System.Threading;
using Core.Utils.Contracts;

namespace Core.Utils
{
    public class Timer : ITimer
    {
        public static readonly Timer CoreTimer = new Timer { AutoUpdate = true };

        private static readonly long Frequency = Stopwatch.Frequency;

        private long lastTime;

        private bool isLastTimeValid;

        private TimeSpan elapsedTime;
        private TimeSpan actualElapsedTime;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static TimeSpan CounterToTimeSpan(long delta)
        {
            return TimeSpan.FromTicks((delta * 10000000) / Frequency);
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                if (this.AutoUpdate)
                {
                    this.Update();
                }

                return this.elapsedTime;
            }
        }

        public TimeSpan ActualElapsedTime
        {
            get
            {
                if (this.AutoUpdate)
                {
                    this.Update();
                }

                return this.actualElapsedTime;
            }
        }

        public TimeSpan TimeLostToPause
        {
            get
            {
                return this.ActualElapsedTime - this.ElapsedTime;
            }
        }

        public bool AutoUpdate { get; set; }
        public bool IsPaused { get; set; }

        public void Reset()
        {
            this.elapsedTime = TimeSpan.Zero;
            this.actualElapsedTime = TimeSpan.Zero;
        }

        public void Pause()
        {
            this.IsPaused = true;
        }

        public void Resume()
        {
            this.IsPaused = false;
        }

        public TimeSpan Update()
        {
            long time = Stopwatch.GetTimestamp();
            TimeSpan elapsed = TimeSpan.FromTicks(0);

            if (!this.isLastTimeValid)
            {
                this.Reset();
            }
            else
            {
                elapsed = CounterToTimeSpan(time - this.lastTime);
                this.actualElapsedTime += elapsed;
                if (!this.IsPaused)
                {
                    this.elapsedTime += elapsed;
                }
            }

            this.lastTime = time;
            this.isLastTimeValid = true;
            return elapsed;
        }
    }
}
