using System.IO.Abstractions;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.DataEdit;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Persistence;
using Effanville.TradingStructures.Common.Diagnostics;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Portfolio;

public sealed class PortfolioManagerFactory : IPortfolioManagerFactory
{
    private readonly IFileSystem _fileSystem;
    private readonly IReportLogger _reportLogger;
    private readonly ITimerFactory _timerFactory;
    private readonly ILoggerFactory _loggerFactory;

    public PortfolioManagerFactory(IFileSystem fileSystem, IReportLogger reportLogger, ILoggerFactory loggerFactory, ITimerFactory timerFactory)
    {
        _fileSystem = fileSystem;
        _reportLogger = reportLogger;
        _timerFactory = timerFactory;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Create a portfolioManager from a settings object.
    /// </summary>
    public IPortfolioManager LoadFromFile(
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings)
    {
        using (_timerFactory.Create("Loading Portfolio"))
        {
            var portfolio = LoadStartPortfolio(startSettings, _fileSystem, _reportLogger);
            return new PortfolioManager(portfolio, startSettings, constructionSettings, _loggerFactory.CreateLogger<PortfolioManager>());
        }
    }

    private IPortfolio LoadStartPortfolio(PortfolioStartSettings settings, IFileSystem fileSystem, IReportLogger logger)
    {
        var persistence = new PortfolioPersistence(logger);
        IPortfolio portfolio;
        if (!string.IsNullOrWhiteSpace(settings.PortfolioFilePath))
        {
            portfolio = persistence.Load(PortfolioPersistence.CreateOptions(settings.PortfolioFilePath, fileSystem, "1.0.0.0"));
        }
        else
        {
            portfolio = PortfolioFactory.GenerateEmpty();
            var res = portfolio.TryAdd(Account.BankAccount, new NameData(settings.DefaultBankAccName.Company, settings.DefaultBankAccName.Name));
            logger.Info(nameof(PortfolioManager), res.ToString());
            var data = new DailyValuation(settings.StartTime.AddDays(-1), settings.StartingCash);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.DefaultBankAccName, data, data);
        }

        return portfolio;
    }
}