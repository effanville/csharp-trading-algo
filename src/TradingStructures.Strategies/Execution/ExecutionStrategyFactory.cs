using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Strategies.Decision;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Execution;

public sealed class ExecutionStrategyFactory : IExecutionStrategyFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public ExecutionStrategyFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public IExecutionStrategy Create(
        StrategyType strategyType,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem)
        => strategyType switch
        {
            StrategyType.LogExecution => new LogExecutionStrategy(_loggerFactory.CreateLogger<LogExecutionStrategy>(), null),
            StrategyType.ExchangeOpen => new ExchangeOpenCalcExecutionStrategy(_loggerFactory.CreateLogger<ExchangeOpenCalcExecutionStrategy>(), stockExchange, decisionSystem),
            StrategyType.ExchangeEvent => new ExchangeEventExecutionStrategy(
                _loggerFactory.CreateLogger<ExchangeEventExecutionStrategy>(),
                stockExchange,
                decisionSystem),
            _ => throw new ArgumentOutOfRangeException($"StrategyType {strategyType} invalid."),
        };
}