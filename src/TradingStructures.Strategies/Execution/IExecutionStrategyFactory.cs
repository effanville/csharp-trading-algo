using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Strategies.Decision;

namespace Effanville.TradingStructures.Strategies.Execution;

public interface IExecutionStrategyFactory
{
    public IExecutionStrategy Create(
        StrategyType strategyType,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem);
}
