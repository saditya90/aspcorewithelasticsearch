using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebElasticSearch.Models;
using WebElasticSearch.Services;

namespace WebElasticSearch.Pages
{
    public class AddDocModel : PageModel
    {
        private readonly IElasticService<Employee> _employeeService;

        public AddDocModel(IElasticService<Employee> employeeService)
        {
            _employeeService = employeeService;
        }

        [BindProperty]
        public EmpViewModel EmpModel { get; set; }

        public class EmpViewModel
        {
            public string Id { get; set; }
            [Required]
            public string Name { get; set; }
            [Required, EmailAddress]
            public string Email { get; set; }
            [Required]
            public string Phone { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _employeeService.AddDocumentAsync(new Employee
                {
                    Email = EmpModel.Email,
                    Name = EmpModel.Name,
                    PhoneNumber = EmpModel.Phone,
                    Id = EmpModel.Id
                });
            }

            return RedirectToPage();
        }
    }
}
