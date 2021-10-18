using System.Collections.Generic;
using System.Linq;
using FinancialStructures.NamingStructures;

namespace TradingConsole.DecisionSystem.Models
{
    public class DecisionStatus
    {
        private readonly List<Decision> fDecisions = new List<Decision>();
        public void AddDecision(NameData stock, TradeDecision buySell)
        {
            fDecisions.Add(new Decision(stock, buySell));
        }

        public List<Decision> GetBuyDecisions()
        {
            return GetDecisions(TradeDecision.Buy);
        }

        public List<string> GetBuyDecisionsStockNames()
        {
            return GetBuyDecisions().Select(decision => decision.StockName.Name).ToList();
        }

        public List<Decision> GetSellDecisions()
        {
            return GetDecisions(TradeDecision.Sell);
        }

        public List<string> GetSellDecisionsStockNames()
        {
            return GetSellDecisions().Select(decision => decision.StockName.Name).ToList();
        }

        private List<Decision> GetDecisions(TradeDecision buySell)
        {
            List<Decision> output = new List<Decision>();
            foreach (Decision dec in fDecisions)
            {
                if (dec.BuySell == buySell)
                {
                    output.Add(dec);
                }
            }
            return output;
        }

        public override string ToString()
        {
            return $"{fDecisions.Count} decisions.";
        }
    }
}
