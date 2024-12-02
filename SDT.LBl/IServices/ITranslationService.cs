namespace SDT.LBl
{
    public interface ITranslationService
    {
        public Task<TranslationOptionsDto> GetTranslationOptions();
        public Task<List<UserDocumentDto>> GetUserDocuments(long userId);
        public Task RunTranslation(long taskId);
        public Task TranslateDocument(TranslateDocumentDto documentDto);
        public Task<TranslationTasksDto> GetTranslationTasks(long documentId);
    }
}
