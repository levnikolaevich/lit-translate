using Microsoft.AspNetCore.Mvc;
using SDT.LBl;

namespace SDT.LApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] long documentId)
        {
            var document = await _fileService.DownloadFile(documentId);
            return File(document.FileBytes, document.ContentType, document.FileName);
        }
    }
}
