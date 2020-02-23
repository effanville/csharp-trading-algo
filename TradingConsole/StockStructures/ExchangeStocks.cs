using FileSupport;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.IO;

namespace TradingConsole.StockStructures
{
    public class ExchangeStocks
    {
        public double GetValue(NameData name, DateTime date)
        {
            foreach (var stock in Stocks)
            {
                if (stock.Name == name)
                {
                    return stock.GetValue(date);
                }
            }

            return 0.0;
        }

        public List<Stock> Stocks = new List<Stock>();

        public void LoadExchangeStocks(string filePath, ErrorReports reports)
        {
            if (File.Exists(filePath))
            {
                var database = XmlFileAccess.ReadFromXmlFile<ExchangeStocks>(filePath);
                Stocks = database.Stocks;
                return;
            }

            reports.AddReport("Loaded Empty New Database.", Location.Loading);
            Stocks = new List<Stock>();
        }
    }
}
