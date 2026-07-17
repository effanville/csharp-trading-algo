namespace Effanville.TradingStructures.StockMarket;

public sealed class StockMarketAdapterSettings
{
    public const string OptionsName = nameof(StockMarketAdapterSettings);
    /// <summary>
    /// The fixed cost associated with each trade.
    /// </summary>
    public decimal TradeCost { get; set; } = 6;
}
