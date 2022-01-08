using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebElasticSearch.Models;

namespace WebElasticSearch.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
