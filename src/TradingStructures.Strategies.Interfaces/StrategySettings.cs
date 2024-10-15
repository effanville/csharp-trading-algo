using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

namespace Effanville.TradingStructures.Strategies;

public sealed class StrategySettings
{
    public PortfolioStartSettings PortfolioStartSettings { get; }
    public PortfolioConstructionSettings PortfolioConstructionSettings { get; }
    public DecisionSystemSetupSettings DecisionSystemSettings { get; }  
    public ExecutionSettings ExecutionSettings { get; }
    
    public StrategySettings(
        PortfolioStartSettings portfolioStartSettings, 
        PortfolioConstructionSettings portfolioConstructionSettings, 
        DecisionSystemSetupSettings decisionSystemSettings, 
        ExecutionSettings executionSettings)
    {
        PortfolioStartSettings = portfolioStartSettings;
        PortfolioConstructionSettings = portfolioConstructionSettings;
        DecisionSystemSettings = decisionSystemSettings;
        ExecutionSettings = executionSettings;
    }
}