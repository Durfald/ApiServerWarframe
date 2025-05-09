using ApiServerWarframe.Models;

namespace ApiServerWarframe.Extensions
{
    public static class StatisticExtensions
    {
        public static double GetWeightedTrend(this List<Statistic> stats)
        {
            if (stats.Count < 3) return 0;

            var ordered = stats.OrderBy(s => s.Datetime).ToList();

            int n = ordered.Count;
            double sumWeights = 0;
            double sumX = 0;
            double sumY = 0;
            double sumXY = 0;
            double sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                double weight = i + 1;
                double x = i + 1;
                double y = ordered[i].WeightedAvgPrice;

                sumWeights += weight;
                sumX += x * weight;
                sumY += y * weight;
                sumXY += x * y * weight;
                sumX2 += x * x * weight;
            }

            double denominator = sumWeights * sumX2 - sumX * sumX;
            if (denominator == 0) return -1;

            double slope = (sumWeights * sumXY - sumX * sumY) / denominator;
            return Math.Round(slope, 3);
        }
    }
}
