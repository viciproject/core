using System;

namespace Vici.Core.Scheduling
{
    public class CyclicScheduler : Scheduler
    {
        public TimeSpan Interval { get; set; }

        public CyclicScheduler(string scheduleId, TimeSpan interval) : base(scheduleId)
        {
            Interval = interval;
        }

        public override bool ShouldRun()
        {
            DateTime lastRun = LastRun;

            if ((TimeProvider.Now - lastRun) >= Interval)
            {
                LastRun = TimeProvider.Now;
                return true;
            }

            return false;
        }
    }
}