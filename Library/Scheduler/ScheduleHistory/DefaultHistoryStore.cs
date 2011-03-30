using System;
using System.Collections.Generic;

namespace Vici.Core.Scheduling
{
    public class DefaultHistoryStore : IScheduleHistoryStore
    {
        private readonly Dictionary<string, DateTime> _lastRunTimes = new Dictionary<string, DateTime>();

        public DateTime LastRun(string taskId)
        {
            DateTime lastRun;

            return _lastRunTimes.TryGetValue(taskId, out lastRun) ? lastRun : DateTime.MinValue;
        }

        public void SetLastRun(string taskId, DateTime lastRun)
        {
            _lastRunTimes[taskId] = lastRun;
            
        }
    }
}