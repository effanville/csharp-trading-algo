using System;
using StructureCommon.Reporting;
using TradingConsole.InputParser;

namespace TradingConsole.Commands
{
    /// <summary>
    /// Command to display the help documentation.
    /// </summary>
    public sealed class HelpCommand : ICommand
    {
        /// <inheritdoc/>
        public ProgramType CommandName
        {
            get
            {
                return ProgramType.Help;
            }
        }

        /// <inheritdoc/>
        public string[] ExpectedOptions
        {
            get
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public HelpCommand()
        {
        }

        /// <inheritdoc/>
        public void Execute(IReportLogger logger, ConsoleStreamWriter consoleWriter, UserInputOptions inputOptions)
        {
            consoleWriter.Write("");
            consoleWriter.Write("Syntax for query:");
            consoleWriter.Write("TradingConsole.exe ProgramType --<<optionName>> <<parameter>>");
            consoleWriter.Write("");
            consoleWriter.Write("ProgramType   - The type of the program");
            consoleWriter.Write("");
            consoleWriter.Write("Possible Commands:");

            foreach (object command in Enum.GetValues(typeof(ProgramType)))
            {
                consoleWriter.Write(command.ToString());
            }

            consoleWriter.Write("");
            consoleWriter.Write("optionName    - An optional argument to add.");
            consoleWriter.Write("");
            consoleWriter.Write("Possible Options");

            foreach (object tokenType in Enum.GetValues(typeof(TextTokenType)))
            {
                consoleWriter.Write(tokenType.ToString());
            }

            consoleWriter.Write("");
            consoleWriter.Write("parameters   - An optionName is followed by the parameter value for this option.");
            consoleWriter.Write("             - Each option can be specified once.");
            consoleWriter.Write("             - Differing options require different parameters.");
        }
    }
}
