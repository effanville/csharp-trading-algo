using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Options;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;

using System.Text.Json;

namespace Effanville.TradingConsole.Commands.Execution
{
    public sealed partial class SimulationCommand
    {
        public sealed class Settings
        {
            public string StockFilePath { get; private init; }

            public DateTime StartTime { get; private set; }

            public DateTime EndTime { get; private set; }

            public TimeSpan EvolutionIncrement { get; private set; }

            public PortfolioStartSettings PortfolioSettings { get; private set; }

            public PortfolioConstructionSettings PortfolioConstructionSettings { get; private set; }

            public DecisionSystemSetupSettings DecisionSystemSettings { get; private set; }

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
                    Settings? settings = JsonSerializer.Deserialize<Settings>(jsonContents);
                    return settings;
                }
                else
                {
                    Settings settings = new Settings
                    {
                        StockFilePath = options.GetOption<string>(StockFilePathName)?.Value ?? string.Empty
                    };
                    CommandOption<DateTime>? startDate = options.GetOption<DateTime>(StartDateName);
                    settings.StartTime = startDate?.Value ?? new DateTime(2010,01, 01);

                    CommandOption<DateTime>? endDate = options.GetOption<DateTime>(EndDateName);
                    settings.EndTime = endDate?.Value ?? new DateTime(2020,01, 01);
                    CommandOption<TimeSpan>? gap = options.GetOption<TimeSpan>(IncrementName);
                    settings.EvolutionIncrement = gap?.Value ?? new TimeSpan(3000);

                    CommandOption<decimal>? fractionInvest = options.GetOption<decimal>(FractionInvestName);
                    CommandOption<decimal>? startingCash = options.GetOption<decimal>(StartingCashName);
                    CommandOption<string>? portfolioFilePath = options.GetOption<string>(PortfolioFilePathName);
                    CommandOption<string>? decisionSystemSettings = options.GetOption<string>(DecisionSystemStatsName);
                    DecisionSystemSetupSettings? decisionSystemFactorySettings 
                        = JsonSerializer.Deserialize<DecisionSystemSetupSettings>(decisionSystemSettings?.Value ?? "{}");

                    settings.PortfolioSettings = new PortfolioStartSettings(
                        portfolioFilePath?.Value ?? string.Empty, 
                        settings.StartTime,
                        startingCash?.Value ?? 20000m);
                    settings.DecisionSystemSettings = decisionSystemFactorySettings ?? new DecisionSystemSetupSettings(DecisionSystem.FiveDayStatsLeastSquares);
                    settings.PortfolioConstructionSettings = new PortfolioConstructionSettings(fractionInvest?.Value ?? 0.25m);
                    return settings;
                }
            }
        }
    }
}
