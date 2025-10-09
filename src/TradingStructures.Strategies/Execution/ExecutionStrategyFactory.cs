using System;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Strategies.Decision;

namespace Effanville.TradingStructures.Strategies.Execution;

public static class ExecutionStrategyFactory
{
    public static IExecutionStrategy Create(
        StrategyType strategyType,
        IReportLogger logger,
        IDecisionSystem decisionSystem)
        => strategyType switch
        {
            StrategyType.LogExecution => new LogExecutionStrategy(logger),
            StrategyType.ExchangeOpen => new ExchangeOpenCalcExecutionStrategy(logger, decisionSystem),
            StrategyType.ExchangeEvent => new ExchangeEventExecutionStrategy(logger, decisionSystem),
            _ => throw new ArgumentOutOfRangeException($"StrategyType {strategyType} invalid."),
        };
}