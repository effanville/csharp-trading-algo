using System;
using System.Collections.Generic;
using TradingConsole.DecisionSystem.TechnicalAnalysisStats;

namespace TradingConsole.StockStructures
{
    public partial class Stock
    {
        private StockDayPrices DayData(DateTime date)
        {
            int numberValues = Valuations.Count;
            int dayIndex = 0;
            do
            {
                dayIndex++;
            } while (date > Valuations[dayIndex].Time && dayIndex < numberValues);

            LastAccessedValuationIndex = dayIndex - 1;
            return Valuations[dayIndex - 1];
        }

        /// <summary>
        /// Calculates the value of the stock at the time specified.
        /// </summary>
        public double Value(DateTime date, DataStream data = DataStream.Close)
        {
            return Value(DayData(date), data);
        }


        /// <summary>
        /// Calculates the value of the stock at the time specified.
        /// </summary>
        public double Value(int index, DataStream data = DataStream.Close)
        {
            return Value(Valuations[index], data);
        }

        private double Value(StockDayPrices values, DataStream data)
        {
            switch (data)
            {
                case (DataStream.Open):
                    return values.Open;
                case (DataStream.High):
                    return values.High;
                case (DataStream.Low):
                    return values.Low;
                case (DataStream.CloseOpen):
                    return values.Close / values.Open;
                case (DataStream.HighOpen):
                    return values.High / values.Open;
                case (DataStream.LowOpen):
                    return values.Low / values.Open;
                case (DataStream.Close):
                default:
                    return values.Close;
            }
        }

        public List<double> Values(DateTime date, int numberValuesBefore, int numberValuesAfter = 0, DataStream data = DataStream.Close)
        {
            DayData(date);
            List<double> desiredValues = new List<double>();
            for (int index = LastAccessedValuationIndex - numberValuesBefore + 1; index < LastAccessedValuationIndex + numberValuesAfter + 1; index++)
            {
                desiredValues.Add(Value(Valuations[index], data));
            }

            return desiredValues;
        }

        public DateTime EarliestTime()
        {
            return Valuations[0].Time;
        }

        public DateTime LastTime()
        {
            return Valuations[Valuations.Count - 1].Time;
        }
    }
}
