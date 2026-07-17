namespace Effanville.TradingStructures.OrderManagement;

public sealed class OrderManagementSettings
{
    /// <summary>
    /// The fixed cost associated with each trade.
    /// </summary>
    public decimal TradeCost { get; set; } = 6.0m;
}
