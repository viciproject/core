using System;

namespace Vici.Core.Scheduling
{
    public abstract class Scheduler
    {
        private readonly string _schedulerId;
        private IScheduleHistoryStore _historyStore;
        private static IScheduleHistoryStore _defaultHistoryStore = new DefaultHistoryStore();

        private ITimeProvider _timeProvider = new RealTimeProvider();

        public static IScheduleHistoryStore DefaultHistoryStore
        {
            set { _defaultHistoryStore = value; }
        }

        public IScheduleHistoryStore HistoryStore
        {
            get { return _historyStore ?? _defaultHistoryStore; }
            set { _historyStore = value; }
        }

        public abstract bool ShouldRun();

        protected Scheduler(string schedulerId)
        {
            _schedulerId = schedulerId ?? Guid.NewGuid().ToString("N");
        }

        public DateTime LastRun
        {
            get { return HistoryStore.LastRun(_schedulerId); } 
            set { HistoryStore.SetLastRun(_schedulerId, value);}
        }

        public ITimeProvider TimeProvider
        {
            get { return _timeProvider; }
            set { _timeProvider = value; }
        }
    }
}