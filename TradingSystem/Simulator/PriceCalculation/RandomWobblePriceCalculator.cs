using System;

using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

using TradingSystem.Simulator.PriceCalculation;

namespace TradingSystem.Simulator.Implementation
{
    public sealed class RandomWobblePriceCalculator : IPriceCalculator
    {
        private readonly PriceCalculationSettings fSettings;
        public decimal CalculateBuyPrice(DateTime time, IStockExchange exchange, TwoName stock)
        {
            decimal openPrice = exchange.GetValue(stock, time, StockDataStream.Open);

            // we modify the price we buy at from the opening price, to simulate market movement.
            decimal upDown = fSettings.RandomNumbers.Next(0, 100) > 100 * fSettings.UpTickProbability ? 1.0m : -1.0m;
            decimal valueModifier = 1.0m + Convert.ToDecimal(fSettings.UpTickSize) * upDown;
            if (openPrice == decimal.MinValue)
            {
                return decimal.MinValue;
            }
            return openPrice * valueModifier;
        }

        public decimal CalculateSellPrice(DateTime time, IStockExchange exchange, TwoName stock)
        {
            // First calculate price that one sells at.
            // This is the open price of the stock, with a combat multiplier.
            decimal upDown = fSettings.RandomNumbers.Next(0, 100) > 100 * fSettings.UpTickProbability ? 1.0m : -1.0m;
            decimal valueModifier = 1 + Convert.ToDecimal(fSettings.UpTickSize) * upDown;
            return exchange.GetValue(stock, time, StockDataStream.Open) * valueModifier;
        }

        public RandomWobblePriceCalculator(PriceCalculationSettings settings)
        {
            fSettings = settings;
        }
    }
}
