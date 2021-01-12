using System;
using FinancialStructures.StockStructures;

namespace TradingConsole.Statistics
{
    /// <summary>
    /// A container for generic statistic calculations for each stock.
    /// </summary>
    /// <remarks>
    /// This is more for a user to experiment, as using this
    /// will in general be slower than knowing what statistics
    /// are to be used.
    /// </remarks>
    public interface IStockStatistic
    {
        /// <summary>
        /// The number of days prior to the date for which one requires data to calculate the statistic.
        /// </summary>
        int BurnInTime
        {
            get;
        }

        /// <summary>
        /// The statistic type this refers to.
        /// </summary>
        StatisticType TypeOfStatistic
        {
            get;
        }

        /// <summary>
        /// The data of the stock this will calculate the statistic of.
        /// </summary>
        StockDataStream DataType
        {
            get;
        }

        /// <summary>
        /// The procedure to calutate the statistic
        /// </summary>
        /// <param name="date">The date on which to calculate the statistic.</param>
        /// <param name="stock">The stock to calculate for.</param>
        /// <returns></returns>
        double Calculate(DateTime date, Stock stock);
    }
}
