using System;
using System.Collections.Generic;
using System.Timers;

using Effanville.TradingStructures.Common.Scheduling;

using TradingSystem.Time;

namespace TradingSystem
{
    public class Scheduler : IScheduler
    {
        private readonly Timer _timer;
        private readonly IClock _internalClock;
        private readonly object _listLock = new object();
        private readonly PriorityQueue<ScheduleEvent, DateTime> _actionsToRun;

        public Scheduler(IClock clock, int timerDelay = 50)
        {
            _actionsToRun = new PriorityQueue<ScheduleEvent, DateTime>();
            _internalClock = clock;
            _timer = new Timer(timerDelay);
            _timer.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //_timer.Stop();
            var currentTime = _internalClock.UtcNow();
            lock (_listLock)
            {
                ScheduleEvent scheduleEvent = _actionsToRun.Count > 0 ? _actionsToRun.Peek() : null;
                while (scheduleEvent != null && scheduleEvent.TimeToRun <= currentTime)
                {
                    scheduleEvent = _actionsToRun.Dequeue();
                    _ = scheduleEvent.TaskToRun();
                    scheduleEvent = _actionsToRun.Count > 0 ? _actionsToRun.Peek() : null;
                }
            }

            //_timer.Start();
        }

        public void Start() => _timer.Start();

        public void ScheduleNewEvent(Action actionToSchedule, DateTime timeToSchedule)
        {
            lock (_listLock)
            {
                _actionsToRun.Enqueue(new ScheduleEvent(actionToSchedule, timeToSchedule), timeToSchedule);
            }
        }

        public void Stop() => _timer.Stop();
    }
}