using System;
using System.Collections.Generic;

using Effanville.Common.Structure.ReportWriting;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.TradingStructures.Common.Trading;

public sealed class TradeDictionaryBuilder
{
    private readonly Dictionary<DateTime, TradeCollection> _tradeCollectionDictionary = new();

    public TradeDictionaryBuilder Clear()
    {
        _tradeCollectionDictionary.Clear();
        return this;
    }

    /// <summary>
    /// Builds from a md table of the form
    /// |StartDate|EndDate|StockName|TradeType|NumberShares
    /// </summary>
    /// 
    public Dictionary<DateTime, TradeCollection> BuildFromString(string input)
    {
        var invertedTable = TableInverter.InvertTable(DocumentType.Md, input);
        var tradeCollectionBuilder = new TradeCollectionBuilder();
        foreach (var row in invertedTable.TableRows)
        {
            string start = row[0];
            string end = row[1];
            string name = row[2];
            string type = row[3];
            string numberShares = row[4];

            DateTime startTime = DateTime.Parse(start);
            DateTime endTime = DateTime.Parse(end);
            string[] names = name.Split('-');
            NameData nameData = names.Length == 2 ? new NameData(names[0], names[1]) : new NameData("", names[0]);
            TradeType buySell = Enum.Parse<TradeType>(type);
            decimal numShares = decimal.Parse(numberShares);

            if (_tradeCollectionDictionary.TryGetValue(startTime, out var value))
            {
                value.Add(nameData, buySell, numShares);
            }
            else
            {
                _ = tradeCollectionBuilder
                    .Reset(startTime, endTime)
                    .Add(nameData, buySell, numShares);
                _tradeCollectionDictionary[startTime] = tradeCollectionBuilder.GetSingleInstance();
            }
        }
        return _tradeCollectionDictionary;
    }
}