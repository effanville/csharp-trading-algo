using System;

namespace Effanville.TradingSystem;

public sealed class SimulationSettings
{
    public string StockFilePath { get; }
    public DateTime StartTime{ get; }
    public DateTime EndTime{ get; }
    public TimeSpan EvolutionIncrement{ get; }
    
    public SimulationSettings(string stockFilePath, DateTime startTime, DateTime endTime, TimeSpan evolutionIncrement)
    {
        StockFilePath = stockFilePath;
        StartTime = startTime;
        EndTime = endTime;
        EvolutionIncrement = evolutionIncrement;
    }
}