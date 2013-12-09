using System;

namespace Vici.Core.Scheduling
{
    public abstract class Scheduler
    {
        private readonly string _schedulerId;
        private ITimeProvider _timeProvider = new RealTimeProvider();

        public static IScheduleHistoryStore DefaultHistoryStore { private get; set; }
        public IScheduleHistoryStore HistoryStore { get; set; }

        public abstract bool ShouldRun();

        protected Scheduler(string schedulerId)
        {
            _schedulerId = schedulerId ?? Guid.NewGuid().ToString("N");

            HistoryStore = DefaultHistoryStore;
        }

        static Scheduler()
        {
            DefaultHistoryStore = new DefaultHistoryStore();
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