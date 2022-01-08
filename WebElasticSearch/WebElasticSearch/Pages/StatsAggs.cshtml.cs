using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebElasticSearch.Models;
using WebElasticSearch.Services;

namespace WebElasticSearch.Pages
{
    public class StatsAggsModel : PageModel
    {
        private readonly IElasticService<Customer> _customerService;

        public StatsAggsModel(IElasticService<Customer> customerService)
        {
            _customerService = customerService;
        }

        public StatsViewModel StatsModel { get; set; }

        public async Task OnGetAsync()
        {
            await Initilize();
        }

        private async Task Initilize()
        {
            var result = await _customerService.StatsAggregationAsync("cst_stats", "account.balanceValue");
            result.GroupStats = await _customerService.GroupByAsync("country");
            StatsModel = new StatsViewModel
            {
                Average = Math.Round(result.Average ?? 0, 2, MidpointRounding.AwayFromZero),
                Max = Math.Round(result.Max ?? 0, 2, MidpointRounding.AwayFromZero),
                Min = Math.Round(result.Min ?? 0, 2, MidpointRounding.AwayFromZero),
                Count = result.Count,
                Median = Math.Round(result.Median, 2, MidpointRounding.AwayFromZero),
                Sum = Math.Round(result.Sum, 2, MidpointRounding.AwayFromZero),
                Items = result.GroupStats
            };
        }

        public class StatsViewModel
        {
            public double? Average { get; set; }
            public long Count { get; set; }
            public double? Max { get; set; }
            public double? Min { get; set; }
            public double Sum { get; set; }
            public double Median { get; set; }
            public List<GroupStats> Items { get; set; }
        }
    }
}
