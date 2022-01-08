using Bogus;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebElasticSearch.Models;
using WebElasticSearch.Services;

namespace WebElasticSearch.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IElasticService<Employee> _employeeService;
        private readonly IElasticService<Customer> _customerService;

        public ValuesController(IElasticService<Employee> employeeService,
            IElasticService<Customer> customerService)
        {
            _employeeService = employeeService;
            _customerService = customerService;
        }

        [Route("[action]")]
        [HttpGet, ActionName("emptestdata")]
        public IActionResult GetEmployeeTestData()
        {
            var employeeFaker = new Faker<Employee>().CustomInstantiator(ci => new Employee())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.PhoneNumber, f => f.Phone.PhoneNumber());

            return Ok(new { data = employeeFaker.Generate() });
        }

        [Route("[action]")]
        [HttpGet, ActionName("getemployees")]
        public async Task<IActionResult> GetEmployess([FromQuery] GridQueryModel gridQueryModel)
        {
            var (totalRecords, documents) = await _employeeService.GetDocumentsAsync(gridQueryModel);

            var gridResponse = new ApiGridResponse<Employee>(documents, totalRecords);

            return Ok(gridResponse);
        }

        [Route("[action]")]
        [HttpGet, ActionName("csttestdata")]
        public IActionResult GetCustomerTestData()
        {
            var accountFaker = new Faker<AccountDetail>()
                .RuleFor(r => r.AccountName, f => f.Finance.AccountName())
                .RuleFor(r => r.Balance, f => $"{f.Finance.Currency().Symbol}")
                .RuleFor(r => r.BalanceValue, f => f.Finance.Amount());

            var customerFaker = new Faker<Customer>().CustomInstantiator(ci => new Customer())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Country, f => f.Address.Country())
                .RuleFor(r => r.Company, f => f.Company.CompanyName())
                .RuleFor(r => r.Account, f => accountFaker.Generate())
                .RuleFor(r => r.SecurityNo, f => f.Finance.Account(length: 5));
            
            var customer = customerFaker.Generate();
            
            customer.RefreshSymbol();
            
            return Ok(new { data = customer });
        }

        [Route("[action]")]
        [HttpGet, ActionName("getcustomers")]
        public async Task<IActionResult> GetCustomers([FromQuery] GridQueryModel gridQueryModel)
        {
            var (totalRecords, documents) = await _customerService.GetDocumentsAsync(gridQueryModel);

            var gridResponse = new ApiGridResponse<Customer>(documents, totalRecords);

            return Ok(gridResponse);
        }

        [Route("[action]")]
        [HttpPost, ActionName("deleteemployee")]
        public async Task<IActionResult> DeleteEmployee([FromForm] string key) 
        {
            await _employeeService.DeleteDocumentAsync(new Employee(key));
            return Ok();
        }

        [Route("[action]")]
        [HttpPost, ActionName("deletecustomer")]
        public async Task<IActionResult> DeleteCustomer([FromForm] string key)
        {
            await _customerService.DeleteDocumentAsync(new Customer(key));
            return Ok();
        }

        [Route("[action]")]
        [HttpPost, ActionName("updateemployee")]
        public async Task<IActionResult> UpdateEmployee([FromForm] Employee employee) 
        {
            await _employeeService.UpdateDocumentAsync(employee);
            return Ok();
        }
    }
}
