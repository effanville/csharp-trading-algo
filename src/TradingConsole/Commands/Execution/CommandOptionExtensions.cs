using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Console.Options;

namespace Effanville.TradingConsole.Commands.Execution
{
    public static class CommandOptionExtensions
    {
        public static CommandOption<T>? GetOption<T>(this IEnumerable<CommandOption> options, string optionName) 
            => options.FirstOrDefault(option => option.Name == optionName) as CommandOption<T>;
    }
}
