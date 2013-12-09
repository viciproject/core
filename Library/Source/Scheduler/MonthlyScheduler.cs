using System;

namespace Vici.Core.Scheduling
{
    public class MonthlyScheduler : TimeOfDayScheduler
    {
        public bool[] MonthDays { get; private set; }
        
        public MonthlyScheduler(string scheduleId, TimeSpan timeOfDay, params int[] daysOfMonth) : base(scheduleId, timeOfDay)
        {
            MonthDays = new bool[32];

            foreach(int m in daysOfMonth)
                if (m > 0 && m <= 31)
                    MonthDays[m] = true;
        }

        public override bool ShouldRun()
        {
            if (MonthDays[TimeProvider.Now.Day])
                return base.ShouldRun();

            return false;
        }
    }
}