﻿namespace TradingConsole.BuySellSystem
{
    /// <summary>
    /// The type of the trade mechanism to use.
    /// This is a user decision as to how they decide they
    /// want the trading to happen.
    /// </summary>
    public enum TradeMechanismType
    {
        /// <summary>
        /// Trade type that first sells all stocks listed as sell, then
        /// attempts to buy all those listed as buy.
        /// </summary>
        SellAllThenBuy,

        /// <summary>
        /// The type used to interact with Interactive Brokers.
        /// </summary>
        IB
    }
}
