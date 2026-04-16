using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies.Decision.Implementation;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Decision;

/// <summary>
/// Factory for creating a decision system.
/// </summary>
public partial class DecisionSystemFactory : IDecisionSystemFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITimerFactory _timerFactory;

    public DecisionSystemFactory(ILoggerFactory loggerFactory, ITimerFactory timerFactory)
    {
        _loggerFactory = loggerFactory;
        _timerFactory = timerFactory;
    }

    public IDecisionSystem Create(Settings settings)
    {
        using (_timerFactory.Create("Calibrating"))
        {
            return settings.DecisionSystemType switch
            {
                DecisionSystem.BuyAll => new BuyAllDecisionSystem(_loggerFactory.CreateLogger<BuyAllDecisionSystem>()),
                DecisionSystem.ArbitraryStatsLeastSquares => new ArbitraryStatsDecisionSystem(settings, _loggerFactory.CreateLogger<ArbitraryStatsDecisionSystem>()),
                DecisionSystem.ArbitraryStatsLasso => new ArbitraryStatsDecisionSystem(settings, _loggerFactory.CreateLogger<ArbitraryStatsDecisionSystem>()),
                DecisionSystem.ArbitraryStatsRidge => new ArbitraryStatsDecisionSystem(settings, _loggerFactory.CreateLogger<ArbitraryStatsDecisionSystem>()),
                DecisionSystem.FiveDayStatsLasso => new FiveDayStatsDecisionSystem(settings, _loggerFactory.CreateLogger<FiveDayStatsDecisionSystem>()),
                DecisionSystem.FiveDayStatsRidge => new FiveDayStatsDecisionSystem(settings, _loggerFactory.CreateLogger<FiveDayStatsDecisionSystem>()),
                _ => new FiveDayStatsDecisionSystem(settings, _loggerFactory.CreateLogger<FiveDayStatsDecisionSystem>()),
            };
        }
    }
}
