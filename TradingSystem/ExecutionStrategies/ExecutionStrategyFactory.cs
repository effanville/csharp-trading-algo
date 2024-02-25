using System;

using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Time;

using TradingSystem.Decisions;
using TradingSystem.Time;

namespace TradingSystem.ExecutionStrategies;

public static class ExecutionStrategyFactory
{
    public static IExecutionStrategy Create(
        StrategyType strategyType,
        IClock clock,
        IReportLogger logger,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem)
    {
        return strategyType switch
        {
            StrategyType.LogExecution => new LogExecutionStrategy(clock, logger),
            StrategyType.TimeIncrementExecution => new TimeIncrementExecutionStrategy(clock, logger, stockExchange, decisionSystem),
            _ => throw new ArgumentOutOfRangeException($"StrategyType {strategyType} invalid."),
        };
    }
}