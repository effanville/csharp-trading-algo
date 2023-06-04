using System;

using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using FinancialStructures.StockStructures.Implementation;

namespace TradingSystem.PriceSystem.Implementation
{
    /// <summary>
    /// Price service where the price data is retrieved from a file.
    /// </summary>
    public sealed class ExchangeFilePriceService : IPriceService
    {
        private readonly IStockExchange _stockExchange;

        public ExchangeFilePriceService(IStockExchange stockExchange)
        {
            _stockExchange = stockExchange;
        }

        public decimal GetPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetPrice(DateTime time, NameData name) => _stockExchange.GetValue(name, time);

        public StockDay GetCandle(DateTime time, TwoName name) => _stockExchange.GetCandle(name, time);

        public decimal GetBidPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetAskPrice(DateTime time, string ticker) => _stockExchange.GetValue(ticker, time);

        public decimal GetBidPrice(DateTime time, TwoName name) => _stockExchange.GetValue(name, time);

        public decimal GetAskPrice(DateTime time, TwoName name) => _stockExchange.GetValue(name, time);
    }
}