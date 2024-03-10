using System;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Strategies.Decision;

namespace Effanville.TradingStructures.Strategies.Execution;

public static class ExecutionStrategyFactory
{
    public static IExecutionStrategy Create(
        StrategyType strategyType,
        IClock clock,
        IReportLogger logger,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem) 
        => strategyType switch
        {
            StrategyType.LogExecution => new LogExecutionStrategy(clock, logger),
            StrategyType.TimeIncrementExecution => new TimeIncrementExecutionStrategy(clock, logger, stockExchange, decisionSystem),
            _ => throw new ArgumentOutOfRangeException($"StrategyType {strategyType} invalid."),
        };
}