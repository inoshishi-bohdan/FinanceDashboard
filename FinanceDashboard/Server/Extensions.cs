using FinanceDashboard.Shared.DTO;
using FinanceDashboard.Shared.Models;
using System.Globalization;

namespace FinanceDashboard.Server
{
    public static class Extensions
    {
        public static List<ChartData> ToChartData(this IQueryable<StatisticData> items
)
        {
            var monthsIncomes = items.GroupBy(income => income.Date.Month, (key, g) => new
            {
                Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(key),
                Amount = (int)Math.Round(g.Sum(x => x.Amount))
            })
            .ToList();

            var result = new List<ChartData>
            {
                new ChartData { Month = "Jan", Amount = 0 },
                new ChartData { Month = "Feb", Amount = 0 },
                new ChartData { Month = "Mar", Amount = 0 },
                new ChartData { Month = "Apr", Amount = 0 },
                new ChartData { Month = "May", Amount = 0 },
                new ChartData { Month = "Jun", Amount = 0 },
                new ChartData { Month = "Jul", Amount = 0 },
                new ChartData { Month = "Aug", Amount = 0 },
                new ChartData { Month = "Sep", Amount = 0 },
                new ChartData { Month = "Oct", Amount = 0 },
                new ChartData { Month = "Nov", Amount = 0 },
                new ChartData { Month = "Dec", Amount = 0 },
            };

            foreach (var item in monthsIncomes)
            {
                var chartData = result.Find(chartData => chartData.Month == item.Month);
                chartData!.Amount = item.Amount;
            }

            return result;
        }

    }
}
