using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Time;

namespace Effanville.TradingSystem
{
    public class Scheduler : IScheduler
    {
        private readonly Timer _timer;
        private readonly IClock _internalClock;
        private readonly object _listLock = new object();
        private readonly PriorityQueue<ScheduleEvent, DateTime> _actionsToRun = new PriorityQueue<ScheduleEvent, DateTime>();
        private bool _currentlyExecuting;

        public Scheduler(IClock clock, int timerDelay = 50)
        {
            _internalClock = clock;
            _timer = new Timer(timerDelay);
            _timer.Elapsed += OnTimedEvent;
        }

        private async void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            if (_currentlyExecuting)
            {
                return;
            }

            _currentlyExecuting = true;
            _timer.Stop();
            var currentTime = _internalClock.UtcNow();
            var tasksToRun = new List<Func<Task>>();
            lock (_listLock)
            {
                ScheduleEvent? scheduleEvent = _actionsToRun.Count > 0 ? _actionsToRun.Peek() : null;
                while (scheduleEvent != null && scheduleEvent.TimeToRun <= currentTime)
                {
                    scheduleEvent = _actionsToRun.Dequeue();
                    tasksToRun.Add(scheduleEvent.TaskToRun);
                    scheduleEvent = _actionsToRun.Count > 0 ? _actionsToRun.Peek() : null;
                }
            }

            foreach (var task in tasksToRun)
            {
                await task();
            }

            lock (_listLock)
            {                
                ScheduleEvent? nextEvent = _actionsToRun.Count > 0 ? _actionsToRun.Peek() : null;
                if (nextEvent != null)
                {
                    _internalClock.NextEventTime = nextEvent.TimeToRun;
                }
            }

            _currentlyExecuting = false;
            _timer.Start();
        }

        public void Start() => _timer.Start();

        public void ScheduleNewEvent(Action actionToSchedule, DateTime timeToSchedule)
        {
            lock (_listLock)
            {
                _actionsToRun.Enqueue(new ScheduleEvent(actionToSchedule, timeToSchedule.ToUniversalTime()), timeToSchedule.ToUniversalTime());
            }
        }

        public void Stop() => _timer.Stop();
    }
}