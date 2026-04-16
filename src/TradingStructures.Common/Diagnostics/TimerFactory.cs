using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Common.Diagnostics
{
    public sealed class TimerFactory(ILoggerFactory loggerFactory) : ITimerFactory
    {
        private readonly ILoggerFactory _loggerFactory = loggerFactory;

        public Timer Create(string name)
            => new Timer(_loggerFactory.CreateLogger<Timer>(), name);
    }
}