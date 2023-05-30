using System;

using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;

namespace TradingSystem.PriceSystem.Implementation
{
    public sealed class RandomWobblePriceCalculator : IPriceService
    {
        private readonly PriceCalculationSettings _settings;
        private readonly IStockExchange _stockExchange;

        public RandomWobblePriceCalculator(PriceCalculationSettings settings, IStockExchange stockExchange)
        {
            _settings = settings;
            _stockExchange = stockExchange;
        }

        public decimal GetPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetPrice(DateTime time, NameData name) => _stockExchange.GetValue(name, time);

        public StockDay GetCandle(DateTime time, TwoName name) => _stockExchange.GetCandle(name, time);

        public decimal GetAskPrice(DateTime time, string ticker)
            => ModifyPrice(_stockExchange.GetValue(ticker, time, StockDataStream.Open), _settings);

        public decimal GetAskPrice(DateTime time, TwoName stock)
            => ModifyPrice(_stockExchange.GetValue(stock, time, StockDataStream.Open), _settings);

        public decimal GetBidPrice(DateTime time, string ticker)
            => ModifyPrice(_stockExchange.GetValue(ticker, time, StockDataStream.Open), _settings);

        public decimal GetBidPrice(DateTime time, TwoName stock)
            => ModifyPrice(_stockExchange.GetValue(stock, time, StockDataStream.Open), _settings);

        private static decimal ModifyPrice(decimal price, PriceCalculationSettings settings)
        {
            // we modify the price we buy at from the opening price, to simulate market movement.
            decimal upDown = settings.RandomNumbers.Next(0, 100) > 100 * settings.UpTickProbability ? 1.0m : -1.0m;
            decimal valueModifier = 1.0m + Convert.ToDecimal(settings.UpTickSize) * upDown;
            if (price == decimal.MinValue)
            {
                return decimal.MinValue;
            }
            return price * valueModifier;
        }
    }
}