using System;
using System.Collections.Generic;

namespace Vici.Core.Scheduling
{
    public class DefaultHistoryStore : IScheduleHistoryStore
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, DateTime> _lastRunTimes = new Dictionary<string, DateTime>();

        public DateTime LastRun(string taskId)
        {
            DateTime lastRun;

            lock (_lock)
                return _lastRunTimes.TryGetValue(taskId, out lastRun) ? lastRun : DateTime.MinValue;
        }

        public void SetLastRun(string taskId, DateTime lastRun)
        {
            lock (_lock)
                _lastRunTimes[taskId] = lastRun;
        }
    }
}