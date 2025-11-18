using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodeCorrectorAI.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string InputCode { get; set; } = string.Empty;
        public string OutputCode { get; set; } = string.Empty;
        
        private readonly IHttpClientFactory _httpClientFactory;
        
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        
        public void OnGet()
        {
        }   

        public async Task<IActionResult> OnPostAsync()
        {
            return Page();
        }
    }
}
