﻿using FileSupport;
using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TradingConsole.InputParser;

namespace TradingConsole.StockStructures
{
    public enum DownloadType
    {
        All,
        Latest
    }

    public class ExchangeStocks
    {
        public double GetValue(NameData name, DateTime date)
        {
            foreach (var stock in Stocks)
            {
                if (stock.Name == name)
                {
                    return stock.Value(date);
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
        private List<Stock> fStocks = new List<Stock>();

        public List<Stock> Stocks
        {
            get { return fStocks; }
            set { fStocks = value; }
        }

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

        public void Download(DownloadType downloadType)
        {
            var reports = new ErrorReports();
            foreach (var stock in Stocks)
            {
                Uri downloadUrl;
                if (downloadType == DownloadType.All)
                {
                    downloadUrl = new Uri(stock.Name.Url);
                }
                else
                {
                    downloadUrl = new Uri(stock.Name.Url); 
                }
                string stockWebsite = DataUpdater.DownloadFromURL(downloadUrl.ToString(), reports).Result;
                ProcessAndAddData(downloadType, stock, stockWebsite);
            }
        }

        private int DateToYahooInt(DateTime date)
        {
            throw new NotImplementedException();
        }

        private DateTime YahooIntToDate(int yahooInt)
        {
            throw new NotImplementedException();
        }

        private void ProcessAndAddData(DownloadType download, Stock stock, string websiteHtml)
        {
            if (download == DownloadType.All)
            { }

            if (download == DownloadType.Latest)
            {
                double close = FindAndGetSingleValue(websiteHtml, "<span class=\"Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)\" data-reactid=\"34\">");
                double open = FindAndGetSingleValue(websiteHtml, "data-test=\"OPEN-value\" data-reactid=\"46\"><span class=\"Trsdu(0.3s) \" data-reactid=\"47\">");
                var range = FindAndGetDoubleValues(websiteHtml, "data-test=\"DAYS_RANGE-value\" data-reactid=\"61\">");
                double volume = FindAndGetSingleValue(websiteHtml, "data-test=\"TD_VOLUME-value\" data-reactid=\"69\"><span class=\"Trsdu(0.3s) \" data-reactid=\"70\">");

                DateTime date = DateTime.Now.TimeOfDay > new DateTime(2010, 1, 1, 16, 30, 0).TimeOfDay ? DateTime.Today : DateTime.Today.AddDays(-1);
                stock.AddValue(date, open, range.Item2, range.Item1, close, volume);
            }
        }

        private double FindAndGetSingleValue(string searchString, string findString, int containedWithin = 50)
        {
            bool continuer(char c)
            {
                if (char.IsDigit(c) || c == '.' || c == ',')
                {
                    return true;
                }

                return false;
            };

            int index = searchString.IndexOf(findString);

            string value = searchString.Substring(index + findString.Length, 40);

            var digits = value.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

            var str = new string(digits);
            if (string.IsNullOrEmpty(str))
            {
                return double.NaN;
            }
            return double.Parse(str);
        }

        private Tuple<double, double> FindAndGetDoubleValues(string searchString, string findString, int containedWithin = 50)
        {
            bool continuer(char c)
            {
                if (char.IsDigit(c) || c == '.' || c == ',')
                {
                    return true;
                }

                return false;
            };

            int index = searchString.IndexOf(findString);

            string value = searchString.Substring(index + findString.Length, 40);

            var digits = value.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

            var str = new string(digits);
            if (string.IsNullOrEmpty(str))
            {
                return new Tuple<double, double>(double.NaN, double.NaN);
            }
            double value1 = double.Parse(str);
            int separator = value.IndexOf("-");
            var secondDigits = value.Substring(separator).SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();
            var str2 = new string(secondDigits);
            double value2 = double.Parse(str2);
            return new Tuple<double, double>(value1, value2);
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