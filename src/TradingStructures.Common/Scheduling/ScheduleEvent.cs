using System;
using System.Threading.Tasks;

namespace Effanville.TradingStructures.Common.Scheduling;

public sealed class ScheduleEvent : IComparable<ScheduleEvent>
{
    public Func<Task> TaskToRun;
    public DateTime TimeToRun;

    public ScheduleEvent(Action action, DateTime time)
    {
        TaskToRun = async () => await Task.Run(action);
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