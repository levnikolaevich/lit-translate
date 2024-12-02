using SDT.Data.Models;

namespace SDT.Repositories
{
    public interface ITranslationRepository
    {
        public Task<List<AICompany>> GetAICompany();
        public Task<List<AILanguageModel>> GetAILanguageModels();
        public Task<AILanguageModel> GetAILanguageModelById(int id, Func<IQueryable<AILanguageModel>, IQueryable<AILanguageModel>>? includeChain = null);
        public Task<List<Language>> GetLanguages();
    }
}
