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
        private readonly Timer? _timer;
        private readonly IClock _internalClock;
        private readonly object _listLock = new object();
        private readonly PriorityQueue<ScheduleEvent, DateTime> _actionsToRun = new PriorityQueue<ScheduleEvent, DateTime>();
        private bool _currentlyExecuting;
        private bool _isRunning;

        public Scheduler(IClock clock, int timerDelay = -1)
        {
            _internalClock = clock;
            if (timerDelay > 0)
            {
                _timer = new Timer(timerDelay);
                _timer.Elapsed += OnTimedEvent;
            }
        }

        private async void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            if (_currentlyExecuting)
            {
                return;
            }

            _currentlyExecuting = true;
            _timer?.Stop();
            await ProcessEvents();

            _currentlyExecuting = false;
            _timer?.Start();
        }

        private async Task ProcessEvents()
        {
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
        }

        public void Start()
        {
            _isRunning = true;
            if(_timer != null)
            {
                _timer.Start();
            }
            else
            {
                EventBasedUpdate();
            }
        }

        private async Task EventBasedUpdate()
        {
            while (_isRunning)
            {
                _currentlyExecuting = true;
                await ProcessEvents();
                _currentlyExecuting = false;
            }
        }

        public void ScheduleNewEvent(Action actionToSchedule, DateTime timeToSchedule)
        {
            lock (_listLock)
            {
                _actionsToRun.Enqueue(new ScheduleEvent(actionToSchedule, timeToSchedule.ToUniversalTime()), timeToSchedule.ToUniversalTime());
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _timer?.Stop();
        }
    }
}