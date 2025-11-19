using CodeCorrectorAI.Models;
using CodeCorrectorAI.Pages;
using CodeCorrectorAI.Services;
using System.Net.Http;

namespace CodeCorrectorAI.Repositories
{
    public class GeminiService : IGeminiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string prompt;

        public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            prompt = $@"
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
            ";
        }

        public async Task<string> AnaliseDeCodigoAsync(string code)
        {
            var geminiApiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(geminiApiKey))
            {
                throw new InvalidOperationException("Gemini APIKey não configurada.");
            }
            var textoInteiro = prompt + "\n\n" + code;
            var requestPayLoad = GeminiRequest.CriarApartirDoTexto(textoInteiro);
            var jsonPayLoad = System.Text.Json.JsonSerializer.Serialize(requestPayLoad);
            var content = new StringContent(jsonPayLoad, System.Text.Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory.CreateClient(); // Cliente HTTP

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
                    return geminiResponse.Candidates[0].Content.Parts[0].Text;
                }
                throw new InvalidOperationException("API retornou uma resposta bem sucedida, mas sem conteúdo.");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Status Code: {response.StatusCode}\n, Content: {errorContent} --- JSON enviado: {jsonPayLoad}");
            }
        }
    }
}
