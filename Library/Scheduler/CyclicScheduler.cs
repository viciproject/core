using System;

namespace Vici.Core.Scheduling
{
    public class CyclicScheduler : Scheduler
    {
        private TimeSpan _interval;

        public CyclicScheduler(string scheduleId, TimeSpan interval) : base(scheduleId)
        {
            Interval = interval;
        }

        public TimeSpan Interval
        {
            get { return _interval; }
            set { _interval = value; }
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