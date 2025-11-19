using CodeCorrectorAI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;

namespace CodeCorrectorAI.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string InputCode { get; set; } = string.Empty;
        [BindProperty]
        public string OutputCode { get; set; } = string.Empty;

        private readonly IGeminiService _geminiService;
        private readonly IExportFileService _exportFileService;
        public IndexModel(IGeminiService geminiService, IExportFileService exportFileService)
        {
            _geminiService = geminiService;
            _exportFileService = exportFileService;
        }
        public void OnGet() {}

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(InputCode))
            {
                ModelState.AddModelError(string.Empty, "Favor, insira o código para análise");
                return Page();
            }

            try
            {
                OutputCode = await _geminiService.AnaliseDeCodigoAsync(InputCode);

            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro de configuração {ex.Message}");
                OutputCode = "Não foi possível processar a requisição devido a um erro de configuração.";
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro na requisição HTTP: {ex.Message}");
                OutputCode = "Não foi possível processar a requisição devido a um erro na comunicação com o serviço.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro inesperado: {ex.Message}");
                OutputCode = "Ocorreu um erro inesperado ao processar a requisição.";
            }
            return Page();
        }

        public IActionResult OnPostExportarAsync ()
        {
            if (string.IsNullOrEmpty(OutputCode))
            {
                ModelState.AddModelError(string.Empty, "Não há código analisado para exportar.");
                return Page();
            }

            var (fileContents, contentType, fileName) = _exportFileService.CriarArquivoMarkdown(OutputCode);
            
            return File(fileContents, contentType, fileName);
        }
    }

    
}
