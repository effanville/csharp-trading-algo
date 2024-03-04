using System;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Exchanges;

namespace Effanville.TradingStructures.Pricing.Implementation
{
    internal sealed class RandomWobblePriceCalculator : IPriceService
    {
        private readonly PriceCalculationSettings _settings;
        private readonly IStockExchange _stockExchange;
        private readonly IScheduler _scheduler;

        public string Name => throw new NotImplementedException();

        public event EventHandler<PriceUpdateEventArgs> PriceChanged;

        public RandomWobblePriceCalculator(PriceCalculationSettings settings, IStockExchange stockExchange, IScheduler scheduler)
        {
            _settings = settings;
            _stockExchange = stockExchange;
            _scheduler = scheduler;
        }

        public void Initialize(EvolverSettings settings)
        {
            var startTime = settings.StartTime;
            var endTime = settings.EndTime;
            foreach (var stock in _stockExchange.Stocks)
            {
                foreach (var valuation in stock.Valuations)
                {
                    if (valuation.Start > startTime && valuation.Start < endTime)
                    {
                        var updateArgs = new PriceUpdateEventArgs(valuation.Start, new StockInstrument(stock), valuation.Open, valuation.CopyAsOpenOnly());
                        _scheduler.ScheduleNewEvent(() => RaisePriceChanged(null, updateArgs), valuation.Start);
                    }
                    if (valuation.Start > startTime && valuation.End < endTime)
                    {
                        var updateArgs = new PriceUpdateEventArgs(valuation.End, new StockInstrument(stock), valuation.Close, valuation);
                        _scheduler.ScheduleNewEvent(() => RaisePriceChanged(null, updateArgs), valuation.End);
                    }
                }
            }
        }

        public void Restart() => throw new NotImplementedException();
        public void Shutdown() { }
        private void RaisePriceChanged(object obj, PriceUpdateEventArgs args)
        {
            EventHandler<PriceUpdateEventArgs> handler = PriceChanged;
            if (handler != null)
            {
                handler?.Invoke(obj, args);
            }
        }

        public decimal GetPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetPrice(DateTime time, NameData name) => _stockExchange.GetValue(name, time);

        public StockDay GetCandle(DateTime startTime, DateTime endTime, TwoName name) => _stockExchange.GetCandle(name, startTime);

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