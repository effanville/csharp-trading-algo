using System;

using Effanville.FinancialStructures.Stocks;

namespace Effanville.TradingStructures.Strategies.Decision;

public class DecisionSystemSettings
{        
    /// <summary>
    /// The start time of the simulation. This is the latest of the
    /// user specified time and the suitable start time from the Exchange data.
    /// </summary>
    public DateTime StartTime { get; protected set; }
    
    public DateTime BurnInEnd { get; private set; }
    
    public int NumberStocks { get; private set; }
    
    public IStockExchange Exchange { get; private set; }

    public DecisionSystemSettings(DateTime startTime, DateTime burnInEnd, int numberStocks, IStockExchange exchange)
    {
        StartTime = startTime;
        BurnInEnd = burnInEnd;
        NumberStocks = numberStocks;
        Exchange = exchange;
    }
    
    public void DoesntRequireBurnIn() => BurnInEnd = StartTime;
}