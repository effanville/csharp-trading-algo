using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

using TradingSystem.Time;

namespace TradingSystem
{
    public sealed class ScheduleEvent
    {
        public Func<Task> TaskToRun;
        public DateTime TimeToRun;

        public ScheduleEvent(Action action, DateTime time)
        {
            TaskToRun = () => Task.Run(action);
            TimeToRun = time;
        }

        public ScheduleEvent(Func<Task> task, DateTime time)
        {
            TaskToRun = task;
            TimeToRun = time;
        }
    }

    public class Scheduler
    {
        private readonly Timer _timer;
        private readonly IClock _internalClock;

        private readonly List<ScheduleEvent> _actionsToRun;

        public Scheduler(IClock clock, int timerDelay = 50)
        {
            _actionsToRun = new List<ScheduleEvent>();
            _internalClock = clock;
            _timer = new Timer(timerDelay);
            _timer.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            var currentTime = _internalClock.UtcNow();
            foreach (var scheduleEvent in _actionsToRun.ToList())
            {
                if (currentTime >= scheduleEvent.TimeToRun)
                {
                    _ = scheduleEvent.TaskToRun();
                    _ = _actionsToRun.Remove(scheduleEvent);
                }
            }

            _timer.Start();
        }

        public void Start() => _timer.Start();

        public void ScheduleNewEvent(Action actionToSchedule, DateTime timeToSchedule)
            => _actionsToRun.Add(new ScheduleEvent(actionToSchedule, timeToSchedule));
    }
}