namespace TradingConsole.Statistics
{
    /// <summary>
    /// List of 
    /// </summary>
    public enum StatisticType
    {
        TodayOpen,
        PrevDayOpen,
        PrevTwoOpen,
        PrevThreeOpen,
        PrevFourOpen,
        PrevFiveOpen,
        TodayClose,
        PrevDayClose,
        PrevTwoClose,
        PrevThreeClose,
        PrevFourClose,
        PrevFiveClose,
        PrevDayHigh,
        PrevDayLow,
        /// <summary>
        /// Considered as 5 days of the open.
        /// </summary>
        MovingAverageWeek,
        /// <summary>
        /// Considered as 20 days of the open.
        /// </summary>
        MovingAverageMonth,
        /// <summary>
        /// Considered as 50 days of the open. 
        /// </summary>
        MovingAverageHalfYear,
        RelativeMovingAverageWeekMonth,
        RelativeMovingAverageMonthHalfYear,
        RelativeMovingAverageWeekHalfYear,
        ADXStatTwoWeek,
        StochasticTwoWeek,
        StochasticFourWeek
    }
}
