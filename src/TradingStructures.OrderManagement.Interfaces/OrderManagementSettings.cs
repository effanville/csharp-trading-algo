namespace Effanville.TradingStructures.OrderManagement;

/// <summary>
/// Contains options for
/// </summary>
public sealed class OrderManagementSettings
{
    /// <summary>
    /// The fixed cost associated with each trade.
    /// </summary>
    public decimal TradeCost { get; }

    /// <summary>
    /// Construct an instance.
    /// </summary>
    public OrderManagementSettings(decimal tradeCost)
    {
        TradeCost = tradeCost;
    }

    public static OrderManagementSettings Default() => new OrderManagementSettings(6);
}
