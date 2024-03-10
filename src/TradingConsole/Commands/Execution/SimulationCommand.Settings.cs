using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Options;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;

using Newtonsoft.Json;

namespace Effanville.TradingConsole.Commands.Execution
{
    internal sealed partial class SimulationCommand
    {
        public sealed class Settings
        {
            public string StockFilePath { get; set; }

            public DateTime StartTime { get; set; }

            public DateTime EndTime { get; set; }

            public TimeSpan EvolutionIncrement { get; set; }

            public PortfolioStartSettings PortfolioSettings { get; set; }

            public PortfolioConstructionSettings PortfolioConstructionSettings { get; set; }

            public DecisionSystemFactory.Settings DecisionSystemSettings { get; set; }

            public TradeMechanismSettings TradeMechanismSettings { get; set; }

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
                    Settings settings = new Settings();
                    settings.StockFilePath = options.GetOption<string>(StockFilePathName)?.Value ?? string.Empty;
                    CommandOption<DateTime>? startDate = options.GetOption<DateTime>(StartDateName);
                    settings.StartTime = startDate?.Value ?? new DateTime(2010,01, 01);

                    CommandOption<DateTime>? endDate = options.GetOption<DateTime>(EndDateName);
                    settings.EndTime = endDate?.Value ?? new DateTime(2020,01, 01);
                    CommandOption<TimeSpan>? gap = options.GetOption<TimeSpan>(IncrementName);
                    settings.EvolutionIncrement = gap?.Value ?? new TimeSpan(3000);

                    CommandOption<decimal>? fractionInvest = options.GetOption<decimal>(FractionInvestName);
                    CommandOption<DecisionSystem>? decisionType = options.GetOption<DecisionSystem>(DecisionSystemName);
                    CommandOption<decimal>? startingCash = options.GetOption<decimal>(StartingCashName);
                    CommandOption<string>? portfolioFilePath = options.GetOption<string>(PortfolioFilePathName);
                    CommandOption<List<StockStatisticType>>? decisionSystemStats = options.GetOption<List<StockStatisticType>>(DecisionSystemStatsName);

                    settings.PortfolioSettings = new PortfolioStartSettings(
                        portfolioFilePath?.Value ?? string.Empty, 
                        settings.StartTime,
                        startingCash?.Value ?? 20000m);
                    settings.DecisionSystemSettings = new DecisionSystemFactory.Settings(
                        decisionType?.Value ?? DecisionSystem.FiveDayStatsLeastSquares,
                        decisionSystemStats?.Value, 
                        1.05, 
                        1.0,
                        1);
                    settings.TradeMechanismSettings = TradeMechanismSettings.Default();
                    settings.PortfolioConstructionSettings = new PortfolioConstructionSettings(fractionInvest?.Value ?? 0.25m);
                    return settings;
                }
            }
        }
    }
}
