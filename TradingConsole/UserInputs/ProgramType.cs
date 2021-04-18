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
        /// Simulate the performance of a stock history and trading scheme.
        /// </summary>
        Simulate,

        /// <summary>
        /// Perform actual trades based upon some strategy.
        /// </summary>
        Trade,
    }
}
