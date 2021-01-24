namespace TradingConsole.InputParser
{
    /// <summary>
    /// The type of the program specified by the user.
    /// </summary>
    public enum ProgramType
    {
        /// <summary>
        /// The program does nothing.
        /// </summary>
        Nothing,

        /// <summary>
        /// Download all possible data.
        /// </summary>
        DownloadAll,

        /// <summary>
        /// Download only the latest data.
        /// </summary>
        DownloadLatest,

        /// <summary>
        /// Configure the Stock database from an image file
        /// </summary>
        Configure,

        /// <summary>
        /// Simulate the performance of a stock history and trading scheme.
        /// </summary>
        Simulate,

        /// <summary>
        /// Perform actual trades based upon some strategy.
        /// </summary>
        Trade,

        /// <summary>
        /// Display the help documentation.
        /// </summary>
        Help
    }
}
