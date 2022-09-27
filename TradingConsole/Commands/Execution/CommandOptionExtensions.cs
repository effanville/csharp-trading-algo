using System.Collections.Generic;
using System.Linq;

using Common.Console.Options;

namespace TradingConsole.Commands.Execution
{
    public static class CommandOptionExtensions
    {
        public static CommandOption<T> GetOption<T>(this IList<CommandOption> options, string optionName)
        {
            return options.FirstOrDefault(option => option.Name == optionName) as CommandOption<T>;
        }
    }
}
