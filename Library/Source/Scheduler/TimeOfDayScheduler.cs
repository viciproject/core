using System;

namespace Vici.Core.Scheduling
{
    public class TimeOfDayScheduler : Scheduler
    {
        public TimeSpan TimeOfDay { get; set; }

        public TimeOfDayScheduler(string scheduleId, TimeSpan timeOfDay) : base(scheduleId)
        {
            TimeOfDay = timeOfDay;
        }

        public override bool ShouldRun()
        {
            DateTime lastRun = LastRun;

            if (lastRun < (TimeProvider.Now.Date.AddDays(-1) + TimeOfDay))
                lastRun = (TimeProvider.Now.Date.AddDays(-1) + TimeOfDay);

            DateTime nextRun = lastRun.Date + TimeOfDay;

            if (lastRun.TimeOfDay >= TimeOfDay)
                nextRun += new TimeSpan(24, 0, 0);

            if (TimeProvider.Now >= nextRun)
            {
                LastRun = TimeProvider.Now;
                return true;
            }

            return false;
        }
    }
}