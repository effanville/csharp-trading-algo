﻿using System;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;

namespace Effanville.TradingStructures.Strategies.Portfolio
{
    /// <summary>
    /// An interface dealing with the construction of a portfolio. For example
    /// an implementor could try to create a Markowitz portfolio.
    /// </summary>
    public interface IPortfolioManager : IService
    {
        /// <summary>
        /// Various settings used in the initial setup of the portfolio.
        /// </summary>
        PortfolioStartSettings StartSettings
        {
            get;
        }

        /// <summary>
        /// Various settings for the construction of a portfolio.
        /// </summary>
        PortfolioConstructionSettings PortfolioConstructionSettings
        {
            get;
        }

        /// <summary>
        /// The portfolio at the start of the simulation.
        /// </summary>
        public IPortfolio StartPortfolio
        {
            get;
        }

        /// <summary>
        /// The portfolio being constructed.
        /// </summary>
        IPortfolio Portfolio
        {
            get;
        }

        /// <summary>
        /// Validate that the following trade is suitable, and potentially alter
        /// the trade output.
        /// </summary>
        Trade? ValidateTrade(DateTime time, Trade trade, IPriceService priceService);

        /// <summary>
        /// Return the amount of money available to trade.
        /// </summary>
        decimal AvailableFunds(DateTime time);

        /// <summary>
        /// Adds a trade into the portfolio.
        /// </summary>
        bool AddTrade(DateTime time, Trade trade, SecurityTrade? tradeConfirmation);

        /// <summary>
        /// Method that is called at the point of a price change occurring.
        /// </summary>
        void OnPriceUpdate(object? obj, PriceUpdateEventArgs eventArgs);

        /// <summary>
        /// Report the status of the portfolio at the time specified.
        /// </summary>
        void ReportStatus(DateTime time);

        /// <summary>
        /// Update the data stored in the portfolio.
        /// </summary>
        void UpdateData(DateTime day, IStockExchange exchange);
    }
}