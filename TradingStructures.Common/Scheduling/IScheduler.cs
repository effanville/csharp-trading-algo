namespace Effanville.TradingStructures.Common.Scheduling;

public interface IScheduler
{
    void Start();
    void ScheduleNewEvent(Action actionToSchedule, DateTime timeToSchedule);
    void Stop();
}