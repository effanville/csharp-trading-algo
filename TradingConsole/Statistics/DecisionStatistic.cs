using System;
using System.Collections.Generic;

namespace TradingConsole.Statistics
{
    public class DecisionStatistic
    {
        public DateTime Time;

        public List<string> BuyStocks;
        public List<string> SellStocks;

        public DecisionStatistic(DateTime time, List<string> buys, List<string> sells)
        {
            Time = time;
            BuyStocks = buys;
            SellStocks = sells;
        }

        public DecisionStatistic(DateTime time)
        {
            Time = time;
        }
    }
}
