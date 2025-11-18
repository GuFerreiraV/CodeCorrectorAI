using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CodeCorrectorAI.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string InputCode { get; set; } = string.Empty;
        public string OutputCode { get; set; } = string.Empty;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _configuration;



        public IndexModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }


        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var geminiApiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(geminiApiKey))
            {
                ModelState.AddModelError(string.Empty, "Gemini APIKey não configurada.");
                return Page();
            }
            var httpClient = _httpClientFactory.CreateClient(); // Cliente HTTP
            var prompt = $@"
            Você é um Engenheiro de Software Sênior e Especialista em Análise de Qualidade de Código. 
            Seu público-alvo são Estudantes Avançados de Programação e Desenvolvedores Juniores que buscam não apenas a solução, mas a compreensão profunda do problema e das boas práticas.
            Instrução Primária: Sua única função é analisar o texto de entrada. Você deve ser capaz de identificar a linguagem de programação (se aplicável) e fornecer uma análise completa.
            Estrutura da Resposta Ideal: A sua resposta deve ser estruturada em três seções obrigatórias e detalhadas, garantindo objetividade e completude:
            
            1. Código Corrigido e Otimizado
            Apresente o código de entrada totalmente corrigido. Além de corrigir erros de sintaxe (indentação, pontuação) e lógica, 
            você deve aplicar otimizações de Boas Práticas da Linguagem (Ex: uso de list comprehensions em Python, early returns, nomenclatura padrão) para demonstrar a versão mais idiomática e eficiente do código. 
            Use um bloco de código Markdown com a linguagem especificada.
            
            2. Análise Detalhada das Correções (A Estrutura de Tópicos)
            Forneça uma análise minuciosa de todas as mudanças realizadas. Use tópicos para cada correção ou otimização, categorizando o tipo de problema.
            A. Erros de Lógica: Descreva o bug original, explique por que ele falhava ou produzia resultados incorretos e detalhe a solução implementada. Evite termos genéricos; use nomenclatura técnica precisa (ex: ""Condição de loop mal definida,"" ""Escopo de variável incorreto"").
            B. Erros de Sintaxe/Identação: Liste e explique correções como parênteses ausentes ou identação incorreta.  
            C. Otimizações e Boas Práticas: Explique as melhorias que tornaram o código mais limpo, legível, manutenível ou eficiente (Ex: ""Refatoração para Single Responsibility Principle"").
            
            3. Verificação de Entrada (Se Não For Código)
            Condição de Exceção: Se o texto de entrada não for código de programação reconhecível, retorne APENAS a seguinte mensagem de erro formatada. Não adicione prefixos ou qualquer outra explicação: Não foi possível realizar a análise.
            O texto fornecido não foi identificado como um código de programação válido (código com sintaxe, classes, funções ou laços).
            "; // Prompt Para a IA

            // Corpo da requisição
            var requestPayLoad = new GeminiRequest
            {
                Contents = new List<GeminiRequestContent>
                {
                    new GeminiRequestContent
                    {
                        Parts = new List<GeminiRequestPart>
                        {
                            new GeminiRequestPart
                            {
                                Text = prompt + "\n\n" + InputCode
                            }
                        }
                    }
                }
            };
            var jsonPayLoad = System.Text.Json.JsonSerializer.Serialize(requestPayLoad);
            var content = new StringContent(jsonPayLoad, System.Text.Encoding.UTF8, "application/json");

            // URL do endpoint Gemini
            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={geminiApiKey}";
            var response = await httpClient.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var geminiResponse = System.Text.Json.JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                if (geminiResponse?.Candidates[0].Content?.Parts != null &&
                    geminiResponse.Candidates[0].Content.Parts.Any())
                {
                    OutputCode = geminiResponse.Candidates[0].Content.Parts[0].Text;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Erro");
                OutputCode = $"Status Code: {response.StatusCode}, Content: {errorContent}";
                //ModelState.AddModelError(string.Empty, "Erro ao chamar a API Gemini.");
            }

            return Page();
        }
    }

    public class GeminiRequestPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
    public class GeminiResponsePart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
    public class GeminiRequestContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiRequestPart> Parts { get; set; }
    }
    public class GeminiResponseContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiRequestPart> Parts { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class Candidate
    {
       [JsonPropertyName("content")]
        public GeminiResponseContent Content { get; set; }
    }

    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public List<GeminiRequestContent> Contents { get; set; }
    }
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }
    }
}
