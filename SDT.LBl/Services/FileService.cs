using SDT.LBl.IServices;
using SDT.Repositories;

namespace SDT.LBl
{
    public class FileService : IFileService
    {
        private readonly IDocumentRepository _documentRepository;


        public FileService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<FileDownloadDto> DownloadFile(long documentId)
        {
            var document = await _documentRepository.GetDocumentById(documentId);
            var fileBytes = await File.ReadAllBytesAsync(document.FilePath);
            var fileName = document.Name;

            return new FileDownloadDto
            {
                FileBytes = fileBytes,
                FileName = fileName,
                ContentType = document.MimeType
            };
        }
    }

    public class FileDownloadDto
    {
        public required byte[] FileBytes { get; set; } // Массив байтов файла
        public required string ContentType { get; set; } = "application/octet-stream"; // MIME-тип файла
        public required string FileName { get; set; } // Имя файла
    }
}