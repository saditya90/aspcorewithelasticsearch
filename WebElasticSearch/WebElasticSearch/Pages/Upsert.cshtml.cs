using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebElasticSearch.Models;
using WebElasticSearch.Services;

namespace WebElasticSearch.Pages
{
    public class UpsertModel : PageModel
    {
        private readonly IElasticService<Customer> _customerService;

        public UpsertModel(IElasticService<Customer> customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public CustomerViewModel CustomerModel { get; set; }

        public async Task OnGetAsync()
        {
            await ValidateIndex();
        }

        private async Task ValidateIndex()
        { 
            var status = await _customerService.IsIndexExists();
            if (!status)
                await _customerService.CreateIndexAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _customerService.UpsertDocumentAsync(new Customer
                {
                    Name = CustomerModel.Name,
                    Country = CustomerModel.Country,
                    Company = CustomerModel.Company,
                    SecurityNo = CustomerModel.SecurityNo,
                    Id = CustomerModel.Id,
                    Account = new AccountDetail
                    {
                        AccountName = CustomerModel.Account.AccountName,
                        Balance = CustomerModel.Account.Balance,
                        BalanceValue = CustomerModel.Account.BalanceValue
                    }
                });
            }

            return RedirectToPage();
        }

        public class CustomerViewModel
        {
            [Required]
            public string Id { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string SecurityNo { get; set; }
            [Required]
            public string Company { get; set; }
            [Required]
            public string Country { get; set; }
            public AccountViewModel Account { get; set; }
        }

        public class AccountViewModel
        {
            [Required]
            public string AccountName { get; set; }
            [Required]
            public string Balance { get; set; }
            public decimal BalanceValue { get; set; }
        }
    }
}
