using FinanceDashboard.Shared.DTO;
using FinanceDashboard.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FinanceDashboard.Server
{
    public static class Extensions
    {
        public static async Task<List<ChartData>> ToChartDataAsync(this IQueryable<StatisticData> items
)
        {
            var monthsIncomes = await items.GroupBy(income => income.Date.Month, (key, g) => new
            {
                Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(key),
                Amount = (int)Math.Round(g.Sum(x => Math.Round(x.Amount, 2)))
            })
            .ToListAsync();

            var result = new List<ChartData>
            {
                new ChartData { Name = "Jan", Amount = 0 },
                new ChartData { Name = "Feb", Amount = 0 },
                new ChartData { Name = "Mar", Amount = 0 },
                new ChartData { Name = "Apr", Amount = 0 },
                new ChartData { Name = "May", Amount = 0 },
                new ChartData { Name = "Jun", Amount = 0 },
                new ChartData { Name = "Jul", Amount = 0 },
                new ChartData { Name = "Aug", Amount = 0 },
                new ChartData { Name = "Sep", Amount = 0 },
                new ChartData { Name = "Oct", Amount = 0 },
                new ChartData { Name = "Nov", Amount = 0 },
                new ChartData { Name = "Dec", Amount = 0 },
            };

            foreach (var item in monthsIncomes)
            {
                var chartData = result.Find(chartData => chartData.Name == item.Month);
                chartData!.Amount = item.Amount;
            }

            return result;
        }

    }
}
