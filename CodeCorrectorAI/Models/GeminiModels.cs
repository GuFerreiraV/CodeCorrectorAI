using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace CodeCorrectorAI.Models
{
    // ------------------Gemini Request---------------------------
    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public List<GeminiRequestContent> Contents { get; set; }

        public static GeminiRequest CriarApartirDoTexto (string texto)
        {
            return new GeminiRequest
            {
                Contents = new List<GeminiRequestContent>
                {
                    new GeminiRequestContent
                    {
                        Parts = new List<GeminiRequestPart>
                        {
                            new GeminiRequestPart
                            {
                                Text = texto
                            }
                        }
                    }
                }
            };
        }
    }
    public class GeminiRequestContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiRequestPart> Parts { get; set; }
    }
    public class GeminiRequestPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
    // ------------------Gemini Response---------------------------
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }
    }

    public class GeminiResponseContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiRequestPart> Parts { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class GeminiResponsePart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public GeminiResponseContent Content { get; set; }
    }
}

