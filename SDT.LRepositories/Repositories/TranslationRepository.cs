using Microsoft.EntityFrameworkCore;
using SDT.Data;
using SDT.Data.Models;

namespace SDT.Repositories
{
    public class TranslationRepository(SDTContext context) : BaseRepository(context), ITranslationRepository
    {
        public async Task<List<AICompany>> GetAICompany()
        {
            return await _dbContext.AICompanies.ToListAsync();
        }

        public async Task<AILanguageModel> GetAILanguageModelById(int id, Func<IQueryable<AILanguageModel>, IQueryable<AILanguageModel>>? includeChain = null)
        {
            var query = _dbContext.AILanguageModels.AsQueryable();

            if (includeChain != null)
            {
                query = includeChain(query);
            }

            return await query.Where(d => d.Id == id && d.DeletedDate == null)
                              .FirstAsync();
        }

        public async Task<List<AILanguageModel>> GetAILanguageModels()
        {
            return await _dbContext.AILanguageModels.ToListAsync();
        }

        public async Task<List<Language>> GetLanguages()
        {
            return await _dbContext.Languages.ToListAsync();
        }
    }
}
