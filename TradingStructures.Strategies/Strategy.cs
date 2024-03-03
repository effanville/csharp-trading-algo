using System;
using System.Threading.Tasks;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.MathLibrary.Finance;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

namespace Effanville.TradingStructures.Strategies;

public class Strategy : IStrategy
{
    private readonly IClock _clock;
    private readonly IReportLogger _logger;
    public string Name => nameof(Strategy);
    public IDecisionSystem DecisionSystem { get; }
    public IExecutionStrategy ExecutionStrategy { get; }
    public IPortfolioManager PortfolioManager { get; }

    public Strategy(
        IDecisionSystem decisionSystem, 
        IExecutionStrategy executionStrategy,
        IPortfolioManager portfolioManager,
        IClock clock,
        IReportLogger logger)
    {
        _clock = clock;
        _logger = logger;
        DecisionSystem = decisionSystem;
        ExecutionStrategy = executionStrategy;
        PortfolioManager = portfolioManager;
    }

    public void Initialize(EvolverSettings settings)
    {
        ExecutionStrategy.Initialize(settings);
        PortfolioManager.Initialize(settings);
    }

    public void Restart()
    {
        ExecutionStrategy.Restart();
        PortfolioManager.Restart();
    }

    public void Shutdown()
    {
        ExecutionStrategy.Shutdown();
        PortfolioManager.Shutdown();
        DateTime time = _clock.UtcNow();
        var latestValue = PortfolioManager.Portfolio.TotalValue(Totals.All, time);
        DateTime earliestTime = PortfolioManager.Portfolio.FirstValueDate(Totals.All, null);
        var startValue = PortfolioManager.Portfolio.TotalValue(Totals.All, earliestTime);

        DateTime latestTime = PortfolioManager.Portfolio.LatestDate(Totals.All, null);
        var car = FinanceFunctions.CAR(new DailyValuation(earliestTime, startValue), new DailyValuation(latestTime, latestValue));
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time:yyyy-MM-ddTHH:mm:ss} total value {latestValue:C2}");
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time:yyyy-MM-ddTHH:mm:ss} total CAR {car}");
    }

    public void OnTimeIncrementUpdate(object obj, TimeIncrementEventArgs eventArgs)
    {
        var task = Task.Run(() => ExecutionStrategy.OnTimeIncrementUpdate(obj, eventArgs));
        task.ContinueWith(x => PortfolioManager.ReportStatus(eventArgs.Time));
    }
}