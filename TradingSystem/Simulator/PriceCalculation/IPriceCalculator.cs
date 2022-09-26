using System;

using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

namespace TradingSystem.Simulator.PriceCalculation
{
    /// <summary>
    /// Enables the calculation of the price of a stock at a given time.
    /// </summary>
    public interface IPriceCalculator
    {
        decimal CalculateBuyPrice(DateTime time, IStockExchange exchange, TwoName stock);
        decimal CalculateSellPrice(DateTime time, IStockExchange exchange, TwoName stock);
    }
}
