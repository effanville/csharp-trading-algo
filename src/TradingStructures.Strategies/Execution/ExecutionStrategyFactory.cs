using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Strategies.Decision;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Execution;

public sealed class ExecutionStrategyFactory : IExecutionStrategyFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IStockExchangeFactory _stockExchangeFactory;

    public ExecutionStrategyFactory(ILoggerFactory loggerFactory, IStockExchangeFactory stockExchangeFactory)
    {
        _loggerFactory = loggerFactory;
        _stockExchangeFactory = stockExchangeFactory;
    }

    public IExecutionStrategy Create(
        StrategyType strategyType,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem)
        => strategyType switch
        {
            StrategyType.LogExecution => new LogExecutionStrategy(_loggerFactory.CreateLogger<LogExecutionStrategy>(), null),
            StrategyType.ExchangeOpen => new ExchangeOpenCalcExecutionStrategy(
                _loggerFactory.CreateLogger<ExchangeOpenCalcExecutionStrategy>(),
                _stockExchangeFactory,
                stockExchange,
                decisionSystem),
            StrategyType.ExchangeEvent => new ExchangeEventExecutionStrategy(
                _loggerFactory.CreateLogger<ExchangeEventExecutionStrategy>(),
                _stockExchangeFactory,
                stockExchange,
                decisionSystem),
            _ => throw new ArgumentOutOfRangeException($"StrategyType {strategyType} invalid."),
        };
}