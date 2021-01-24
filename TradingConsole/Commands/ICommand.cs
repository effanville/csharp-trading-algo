using StructureCommon.Reporting;
using TradingConsole.InputParser;

namespace TradingConsole.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// The type of command referred to.
        /// </summary>
        ProgramType CommandName
        {
            get;
        }

        /// <summary>
        /// The expected options allowed for this command.
        /// </summary>
        string[] ExpectedOptions
        {
            get;
        }

        /// <summary>
        /// Execute the given command.
        /// </summary>
        void Execute(IReportLogger logger, ConsoleStreamWriter consoleWriter, UserInputOptions inputOptions);
    }
}
