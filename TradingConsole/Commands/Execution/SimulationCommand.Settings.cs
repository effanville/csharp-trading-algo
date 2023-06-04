using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

using Common.Console.Options;

using FinancialStructures.StockStructures.Statistics;

using Newtonsoft.Json;

using TradingSystem.Decisions;
using TradingSystem.PortfolioStrategies;
using TradingSystem.Trading;

namespace TradingConsole.Commands.Execution
{
    internal sealed partial class SimulationCommand
    {
        public sealed class Settings
        {
            public string StockFilePath
            {
                get;
                set;
            }

            public DateTime StartTime
            {
                get;
                set;
            }

            public DateTime EndTime
            {
                get;
                set;
            }

            public TimeSpan EvolutionIncrement
            {
                get;
                set;
            }

            public PortfolioStartSettings PortfolioSettings
            {
                get;
                set;
            }

            public PortfolioConstructionSettings PortfolioConstructionSettings
            { get; set; }

            public DecisionSystemFactory.Settings DecisionSystemSettings
            {
                get;
                set;
            }

            public TradeMechanismSettings TradeMechanismSettings
            {
                get;
                set;
            }

            private Settings()
            {
            }

            public static Settings CreateSettings(IList<CommandOption> options, IFileSystem fileSystem)
            {
                var jsonPath = options.FirstOrDefault(option => option.Name == "jsonSettingsPath");
                if (jsonPath != null && jsonPath.ValueAsObject != null)
                {
                    string path = jsonPath.ValueAsObject.ToString();
                    string jsonContents = fileSystem.File.ReadAllText(path);
                    Settings settings = JsonConvert.DeserializeObject<Settings>(jsonContents);
                    return settings;
                }
                else
                {
                    var settings = new Settings();
                    settings.StockFilePath = options.GetOption<string>(StockFilePathName).Value;
                    var startDate = options.GetOption<DateTime>(StartDateName);
                    settings.StartTime = startDate.Value;

                    var endDate = options.GetOption<DateTime>(EndDateName);
                    settings.EndTime = endDate.Value;
                    var gap = options.GetOption<TimeSpan>(IncrementName);
                    settings.EvolutionIncrement = gap.Value;

                    var fractionInvest = options.GetOption<decimal>(FractionInvestName);
                    var decisionType = options.GetOption<DecisionSystem>(DecisionSystemName);
                    var startingCash = options.GetOption<decimal>(StartingCashName);
                    var portfolioFilePath = options.GetOption<string>(PortfolioFilePathName);
                    var decisionSystemStats = options.GetOption<List<StockStatisticType>>(DecisionSystemStatsName);

                    settings.PortfolioSettings = new PortfolioStartSettings(portfolioFilePath.Value, startDate.Value, startingCash.Value);
                    settings.DecisionSystemSettings = new DecisionSystemFactory.Settings(decisionType.Value, decisionSystemStats.Value, 1.05, 1.0, 1);
                    settings.TradeMechanismSettings = TradeMechanismSettings.Default();
                    settings.PortfolioConstructionSettings = new PortfolioConstructionSettings(fractionInvest?.Value ?? 0.25m);
                    return settings;
                }
            }
        }
    }
}
