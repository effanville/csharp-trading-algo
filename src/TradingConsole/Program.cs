using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Effanville.Common.Console.DependencyInjection;
using Effanville.FinancialStructures.Persistence;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Persistence;
using Effanville.TradingConsole.Commands.ExchangeCreation;
using Effanville.TradingConsole.Commands.Execution;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Effanville.TradingConsole;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddScoped<IPersistence<IStockExchange>, ExchangePersistence>();
        IHost host = builder.SetupConsole(
                args,
                new List<Type>()
                {
                    typeof(ConfigureCommand),
                    typeof(DownloadCommand),
                    typeof(DownloadAllCommand),
                    typeof(DownloadLatestCommand),
                    typeof(SimulationCommand)
                })
            .Build();
        await host.RunAsync();
    }
}
