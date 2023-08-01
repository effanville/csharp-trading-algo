using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;

using TradingSystem.Time;

namespace TradingSystem
{
    public sealed class ScheduleEvent : IComparable<ScheduleEvent>
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

        public int CompareTo(ScheduleEvent other) => TimeToRun.CompareTo(other.TimeToRun);
        public override string ToString() => TimeToRun.ToString();
    }

    public class Scheduler
    {
        private readonly Timer _timer;
        private readonly IClock _internalClock;
        private readonly object _listLock = new object();
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
            //_timer.Stop();
            var currentTime = _internalClock.UtcNow();
            lock (_listLock)
            {
                ScheduleEvent scheduleEvent = _actionsToRun.Count > 0 ? _actionsToRun[0] : null;
                while (scheduleEvent != null && scheduleEvent.TimeToRun <= currentTime)
                {
                    _ = scheduleEvent.TaskToRun();
                    _ = _actionsToRun.Remove(scheduleEvent);
                    scheduleEvent = _actionsToRun.Count > 0 ? _actionsToRun[0] : null;
                }
            }

            //_timer.Start();
        }

        public void Start() => _timer.Start();

        public void ScheduleNewEvent(Action actionToSchedule, DateTime timeToSchedule)
        {
            lock (_listLock)
            {
                if (_actionsToRun.Count == 0)
                {
                    _actionsToRun.Add(new ScheduleEvent(actionToSchedule, timeToSchedule));
                    return;
                }
                if (timeToSchedule <= _actionsToRun[0].TimeToRun)
                {
                    _actionsToRun.Insert(0, new ScheduleEvent(actionToSchedule, timeToSchedule));
                    return;
                }
                if (timeToSchedule >= _actionsToRun[_actionsToRun.Count - 1].TimeToRun)
                {
                    _actionsToRun.Add(new ScheduleEvent(actionToSchedule, timeToSchedule));
                    return;
                }

                for (int index = 1; index < _actionsToRun.Count; index++)
                {
                    var prevScheduleEvent = _actionsToRun[index - 1];
                    var scheduleEvent = _actionsToRun[index];
                    if (timeToSchedule >= prevScheduleEvent.TimeToRun && timeToSchedule < scheduleEvent.TimeToRun)
                    {
                        _actionsToRun.Insert(index, new ScheduleEvent(actionToSchedule, timeToSchedule));
                        return;
                    }
                }
            }
        }

        public void Stop() => _timer.Stop();
    }
}