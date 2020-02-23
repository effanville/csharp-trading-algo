using FinancialStructures.GUIFinanceStructures;
using System.Collections.Generic;

namespace TradingConsole
{
    public class Decision
    {
        public NameData StockName;
        public StockTradeDecision BuySell;
        public Decision(NameData stock, StockTradeDecision buySell)
        {
            StockName = stock;
            BuySell = buySell;
        }
    }

    public class DecisionStatus
    {
        private List<Decision> decisions;
        public void AddDecision(NameData stock, StockTradeDecision buySell)
        {
            decisions.Add(new Decision(stock, buySell));
        }

        public List<Decision> GetBuyDecisions()
        {
            return GetDecisions(StockTradeDecision.Buy);
        }

        public List<Decision> GetSellDecisions()
        { 
            return GetDecisions(StockTradeDecision.Sell);
        }

        private List<Decision> GetDecisions(StockTradeDecision buySell)
        {
            var output = new List<Decision>();
            foreach (var dec in decisions)
            {
                if (dec.BuySell == buySell)
                {
                    output.Add(dec);
                }
            }
            return output;
        }
    }
}
