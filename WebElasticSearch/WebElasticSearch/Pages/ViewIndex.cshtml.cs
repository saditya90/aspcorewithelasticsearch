using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebElasticSearch.Models;
using WebElasticSearch.Services;

namespace WebElasticSearch.Pages
{
    public class ViewIndexModel : PageModel
    {
        private readonly IElasticService<Employee> _employeeService;
        private readonly IElasticService<Customer> _customerService;

        public ViewIndexModel(IElasticService<Employee> employeeService,
            IElasticService<Customer> customerService)
        {
            _employeeService = employeeService;
            _customerService = customerService;
        }

        [BindProperty]
        public string IndexId { get; set; }
        public IndexDetails MetaData { get; set; }
        public List<SelectListItem> AllIndicies { get; set; }

        public async Task OnGetAsync()
        {
            await Initilize();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await Initilize();

            return Page();
        }

        public async Task<IActionResult> OnGetEmpIndexAsync()
        {
            var status = await _employeeService.IsIndexExists();

            if (!status)
                await _employeeService.CreateIndexAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCstIndexAsync()
        {
            var status = await _customerService.IsIndexExists();

            if (!status)
                await _customerService.CreateIndexAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteIndexAsync(string key) 
        {
            if (!string.IsNullOrEmpty(key)) 
            {
                await _customerService.DeleteIndexAsync(key);
            }

            return RedirectToPage();
        }

        public class IndexDetails
        {
            public string Id { get; set; }
            public DateTime Created { get; set; }
            public string Name { get; set; }
            public string IndexUId { get; set; }
            public int Shards { get; set; }
            public int Replicas { get; set; }
            public long TotalDocs { get; set; }
        }

        private async Task Initilize()
        {
            AllIndicies = new List<SelectListItem>();

            var indexResponse = await _employeeService.GetAllIndicesAsync();

            foreach (var item in indexResponse)
            {
                if (item.Values.TryGetValue("index.provided_name", out var v))
                    AllIndicies.Add(new SelectListItem
                    {
                        Text = v.ToString(),
                        Value = v.ToString(),
                        Selected = string.IsNullOrEmpty(IndexId) && AllIndicies.Count == 0 || v.ToString() == IndexId
                    });
            }


            if (AllIndicies.Any(q => q.Selected))
            {
                var value = AllIndicies.FirstOrDefault(q => q.Selected).Value;

                var index = indexResponse.FirstOrDefault(q => q.Key == value);

                MetaData = new IndexDetails
                {
                    Created = DateTime.Now.GetUnixDate(index?.Values["index.creation_date"]?.ToString()),

                    Name = index?.Values["index.provided_name"]?.ToString(),

                    Shards = 0.GetValue(index?.Values["index.number_of_shards"]?.ToString()),

                    Replicas = 0.GetValue(index?.Values["index.number_of_replicas"]?.ToString()),

                    IndexUId = index?.Values["index.uuid"]?.ToString(),

                    TotalDocs = value switch
                    {
                        "employee" => await _employeeService.GetDocumentsCount(),
                        _ => await _customerService.GetDocumentsCount()
                    },

                    Id = value
                };
            }
        }
    }
}
