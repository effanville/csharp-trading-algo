using System;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Scheduling;

namespace Effanville.TradingStructures.Pricing.Implementation
{
    /// <summary>
    /// Price service where the price data is retrieved from a file.
    /// </summary>
    internal sealed class ExchangeFilePriceService : IPriceService
    {
        private readonly IScheduler? _scheduler;
        private readonly IStockExchange _stockExchange;

        public string Name => nameof(ExchangeFilePriceService);

        public event EventHandler<PriceUpdateEventArgs>? PriceChanged;

        public ExchangeFilePriceService(IStockExchange stockExchange, IScheduler? scheduler)
        {
            _stockExchange = stockExchange;
            _scheduler = scheduler;
        }

        public void Initialize(EvolverSettings settings)
        {
            if (_scheduler == null)
            {
                return;
            }
            var startTime = settings.StartTime;
            var endTime = settings.EndTime;
            foreach (var stock in _stockExchange.Stocks)
            {
                foreach (var valuation in stock.Valuations)
                {
                    if (valuation.Start > startTime && valuation.Start < endTime)
                    {
                        var updateArgs = new PriceUpdateEventArgs(valuation.Start, stock.Name, valuation.Open, valuation.CopyAsOpenOnly());
                        _scheduler.ScheduleNewEvent(() => RaisePriceChanged(null, updateArgs), valuation.Start);
                    }
                    if (valuation.Start > startTime && valuation.End < endTime)
                    {
                        var updateArgs = new PriceUpdateEventArgs(valuation.End, stock.Name, valuation.Close, valuation);
                        _scheduler.ScheduleNewEvent(() => RaisePriceChanged(null, updateArgs), valuation.End);
                    }
                }
            }
        }

        public void Restart() { }
        public void Shutdown() { }

        private void RaisePriceChanged(object? obj, PriceUpdateEventArgs args) => PriceChanged?.Invoke(obj, args);

        public decimal GetPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetPrice(DateTime time, NameData name) => _stockExchange.GetValue(name, time);

        public StockDay GetCandle(DateTime startTime, DateTime endTime, TwoName name) => _stockExchange.GetCandle(name, startTime);

        public decimal GetBidPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetAskPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetBidPrice(DateTime time, TwoName name) => _stockExchange.GetValue(name, time);

        public decimal GetAskPrice(DateTime time, TwoName name) => _stockExchange.GetValue(name, time);
    }
}