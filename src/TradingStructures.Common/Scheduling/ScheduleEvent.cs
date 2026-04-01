using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Effanville.TradingStructures.Common.Scheduling;

public sealed class ScheduleEvent : IComparable<ScheduleEvent>
{
    private static long _globalIdCounter;
    private readonly long _id;
    public readonly Func<Task> TaskToRun;
    public DateTime TimeToRun;

    public ScheduleEvent(Action action, DateTime time)
    {
        _id = Interlocked.Increment(ref _globalIdCounter);
        TaskToRun = async () => await Task.Run(action);
        TimeToRun = time;
    }

    public ScheduleEvent(Func<Task> task, DateTime time)
    {
        _id = Interlocked.Increment(ref _globalIdCounter);
        TaskToRun = task;
        TimeToRun = time;
    }

    public int CompareTo(ScheduleEvent? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (TimeToRun.Equals(other.TimeToRun))
        {
            return _id.CompareTo(other._id);
        }

        return TimeToRun.CompareTo(other.TimeToRun);
    }
    
    public override string ToString() => TimeToRun.ToString(CultureInfo.InvariantCulture);
}