namespace CodeCorrectorAI.Services
{
    public interface IGeminiService
    {
        Task<string> AnaliseDeCodigoAsync(string code);
    }
}
