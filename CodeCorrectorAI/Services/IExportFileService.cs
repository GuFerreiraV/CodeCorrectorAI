namespace CodeCorrectorAI.Services
{
    public interface IExportFileService
    {
        (byte[] fileContents, string contentType, string fileName) CriarArquivoMarkdown (string content);
    }
}
