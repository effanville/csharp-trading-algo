namespace Effanville.TradingStructures.Strategies.Portfolio;

public interface IPortfolioManagerFactory
{
    IPortfolioManager LoadFromFile(
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings);
}