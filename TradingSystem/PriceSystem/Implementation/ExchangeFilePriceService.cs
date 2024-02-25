using System;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Implementation;

using TradingSystem.ExchangeStructures;
using TradingSystem.MarketEvolvers;

namespace TradingSystem.PriceSystem.Implementation
{
    /// <summary>
    /// Price service where the price data is retrieved from a file.
    /// </summary>
    public sealed class ExchangeFilePriceService : IPriceService
    {
        private readonly Scheduler _scheduler;
        private readonly IStockExchange _stockExchange;

        public string Name => throw new NotImplementedException();

        public event EventHandler<PriceUpdateEventArgs> PriceChanged;

        public ExchangeFilePriceService(IStockExchange stockExchange, Scheduler scheduler)
        {
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

        public decimal GetBidPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetAskPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetBidPrice(DateTime time, TwoName name) => _stockExchange.GetValue(name, time);

        public decimal GetAskPrice(DateTime time, TwoName name) => _stockExchange.GetValue(name, time);
    }
}