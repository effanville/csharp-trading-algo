using Effanville.FinancialStructures.Database;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Strategies;

public class StrategyHistory
{
    /// <summary>
    /// The ending <see cref="IPortfolio"/> from the evolution.
    /// </summary>
    public IPortfolio Portfolio { get; }

    /// <summary>
    /// The history of all decisions made during the evolution.
    /// </summary>
    public TradeHistory Decisions { get; }

    /// <summary>
    /// The history of all the trades enacted in the evolution.
    /// </summary>
    public TradeHistory Trades { get; }

    public StrategyHistory()
        : this(PortfolioFactory.GenerateEmpty())
    {
    }

    public StrategyHistory(IPortfolio portfolio)
    {
        Portfolio = portfolio;
        Decisions = new TradeHistory();
        Trades = new TradeHistory();
    }

    public static StrategyHistory NoResult() => new StrategyHistory();
}
