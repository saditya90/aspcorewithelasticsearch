using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebElasticSearch.Models;
using WebElasticSearch.Services;

namespace WebElasticSearch.Pages
{
    public class EmployeesModel : PageModel
    {
        private readonly ILogger<EmployeesModel> _logger;
        private readonly IElasticService<Employee> _employeeService;

        public EmployeesModel(ILogger<EmployeesModel> logger,
            IElasticService<Employee> employeeService)
        {
            _logger = logger; _employeeService = employeeService;
        }

        public async Task OnGetAsync()
        {
            await CheckIndex();
        } 

        private async Task CheckIndex()
        {
            var status = await _employeeService.IsIndexExists();

            if (!status)
                await _employeeService.CreateIndexAsync();
        }
    }
}
