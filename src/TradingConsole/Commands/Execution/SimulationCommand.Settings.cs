using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Options;
using Effanville.TradingStructures.Common;

using Newtonsoft.Json;

namespace Effanville.TradingConsole.Commands.Execution
{
    public sealed partial class SimulationCommand
    {
        public sealed class Settings
        {
            public EvolverSettings EvolverSettings { get; private set; }

            private Settings() { }

            public static Settings? CreateSettings(IList<CommandOption> options, IFileSystem fileSystem)
            {
                CommandOption<string>? jsonPath = options.GetOption<string>("jsonSettingsPath");
                if (jsonPath != null && jsonPath.ValueAsObject != null)
                {
                    string? path = jsonPath.ValueAsObject.ToString();
                    if (string.IsNullOrEmpty(path))
                    {
                        return null;
                    }

                    string jsonContents = fileSystem.File.ReadAllText(path);
                    Settings? settings = JsonConvert.DeserializeObject<Settings>(jsonContents);
                    return settings;
                }
                else
                {
                    var stockFilePath = options.GetOption<string>(StockFilePathName)?.Value ?? string.Empty;
                    Settings settings = new Settings
                    {
                    };
                    CommandOption<DateTime>? startDate = options.GetOption<DateTime>(StartDateName);
                    var startTime = startDate?.Value ?? new DateTime(2010, 01, 01);

                    CommandOption<DateTime>? endDate = options.GetOption<DateTime>(EndDateName);
                    var endTime = endDate?.Value ?? new DateTime(2020, 01, 01);
                    CommandOption<TimeSpan>? gap = options.GetOption<TimeSpan>(IncrementName);
                    var evolutionIncrement = gap?.Value ?? new TimeSpan(3000);

                    settings.EvolverSettings = new EvolverSettings(stockFilePath, startTime, endTime, evolutionIncrement);
                    return settings;
                }
            }
        }
    }
}
