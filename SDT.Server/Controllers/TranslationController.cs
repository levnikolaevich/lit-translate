using Microsoft.AspNetCore.Mvc;
using SDT.LBl;

namespace SDT.LApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationService _translationService;

        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        [HttpGet("get-translation-options")]
        public async Task<TranslationOptionsDto> GetTranslationOptions()
        {
            return await _translationService.GetTranslationOptions();
        }

        [HttpGet("get-user-documents")]
        public async Task<List<UserDocumentDto>> GetUserDocuments()
        {
            return await _translationService.GetUserDocuments(1);
        }

        [HttpPost("translate")]
        public async Task Translate([FromForm] TranslateDocumentRequest request)
        {
            var documentDto = new TranslateDocumentDto
            {
                FileStream = request.Document.OpenReadStream(),
                FileName = request.Document.FileName,
                MimeType = request.Document.ContentType,
                TargetLanguageIds = request.TargetLanguageIds,
                LanguageModelId = request.LanguageModelId,
                ApiKey = request.ApiKey
            };

            await _translationService.TranslateDocument(documentDto);
        }

        [HttpGet("get-translation-tasks")]
        public async Task<TranslationTasksDto> GetTranslationTasks([FromQuery] long documentId)
        {
            return await _translationService.GetTranslationTasks(documentId);
        }

        [HttpPost("run-translation")]
        public async Task RunTranslation([FromBody] long taskId)
        {
            await _translationService.RunTranslation(taskId);
        }

        [HttpGet("download-translation")]
        public async Task DownloadTranslation([FromQuery] long taskId)
        {

            await _translationService.GetUserDocuments(1);
        }

        [HttpGet("update-translation-progress")]
        public async Task UpdateTranslationProgress([FromForm] long taskId)
        {
            await _translationService.GetUserDocuments(1);
        }


    }

    public class TranslateDocumentRequest
    {
        [FromForm] public required IFormFile Document { get; set; }
        [FromForm] public required List<int> TargetLanguageIds { get; set; }
        [FromForm] public required int LanguageModelId { get; set; }
        [FromForm] public required string ApiKey { get; set; }
    }
}
