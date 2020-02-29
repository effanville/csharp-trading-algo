using FileSupport;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.IO;
using TradingConsole.InputParser;

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

        public DateTime EarliestDate()
        {
            DateTime earliest = Stocks[0].EarliestTime();

            for (int stockIndex = 1; stockIndex < Stocks.Count; stockIndex++)
            {
                var stockEarliest = Stocks[stockIndex].EarliestTime();
                if (stockEarliest < earliest)
                {
                    earliest = stockEarliest;
                }
            }

            return earliest;
        }

        public DateTime LastDate()
        {
            DateTime last = Stocks[0].EarliestTime();

            for (int stockIndex = 1; stockIndex < Stocks.Count; stockIndex++)
            {
                var stockLast = Stocks[stockIndex].LastTime();
                if (stockLast < last)
                {
                    last = stockLast;
                }
            }

            return last;
        }

        public List<Stock> Stocks = new List<Stock>();

        public void LoadExchangeStocks(string filePath, ErrorReports reports)
        {
            if (File.Exists(filePath))
            {
                var database = XmlFileAccess.ReadFromXmlFile<ExchangeStocks>(filePath);
                if (database != null)
                {
                    Stocks = database.Stocks;
                }
                return;
            }

            reports.AddReport("Loaded Empty New Database.", Location.Loading);
            Stocks = new List<Stock>();
        }

        public void SaveExchangeStocks(string filePath, ErrorReports reports)
        {
            XmlFileAccess.WriteToXmlFile<ExchangeStocks>(filePath, this);
        }

        public void DownloadAll()
        {

        }

        public void DownloadLatest()
        {

        }

        public void Configure(UserInputOptions inputOptions)
        {
            // here this filepath is location of file to configure from.
            string filePath = inputOptions.StockFilePath;
            string[] fileContents = new string[] { };
            try 
            {
                fileContents = File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read from file located at {filePath}: {ex.Message}");
            }

            if (fileContents.Length == 0)
            {
                Console.WriteLine("Nothing in file selected.");
            }

            foreach (string line in fileContents)
            {
                var inputs = line.Split(',');
                AddStock(inputs);
            }
        }

        private void AddStock(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                Console.WriteLine("Insufficient Data in line to add Stock");
                return;
            }

            Stock stock = new Stock(parameters[0], parameters[1], parameters[2]);
            Stocks.Add(stock);
        }
    }
}
