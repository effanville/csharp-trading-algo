using System.Collections.Generic;
using System.Linq;

using FinancialStructures.NamingStructures;

namespace TradingSystem.Decisions.Models
{
    public sealed class DecisionStatus
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
            return GetBuyDecisions().Select(decision => decision.StockName.ToString()).ToList();
        }

        public List<Decision> GetSellDecisions()
        {
            return GetDecisions(TradeDecision.Sell);
        }

        public List<string> GetSellDecisionsStockNames()
        {
            return GetSellDecisions().Select(decision => decision.StockName.ToString()).ToList();
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Buys: {string.Join(",", GetBuyDecisions().Select(dec => dec.StockName))}. Sells:  {string.Join(",", GetSellDecisions().Select(dec => dec.StockName))}";
        }
    }
}
