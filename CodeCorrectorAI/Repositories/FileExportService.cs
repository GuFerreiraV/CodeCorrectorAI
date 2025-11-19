using CodeCorrectorAI.Services;
using System.Text;

namespace CodeCorrectorAI.Repositories
{
    public class FileExportService : IExportFileService
    {
        public (byte[] fileContents, string contentType, string fileName) CriarArquivoMarkdown (string content)
        {
            // Converte conteúdo para bytes
            var bytes = Encoding.UTF8.GetBytes(content);
        
            // Define tipo de arquivo 
            var contentType = "text/markdown";

            var fileName = "codigo_analisado.md";

            return (bytes, contentType, fileName);
        }
    }
}
