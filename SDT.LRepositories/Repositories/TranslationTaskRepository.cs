using Microsoft.EntityFrameworkCore;
using SDT.Data;
using SDT.Data.Models;

namespace SDT.Repositories
{
    public class TranslationTaskRepository(SDTContext context) : BaseRepository(context), ITranslationTaskRepository
    {
        public async Task AddedTanslationTasks(List<TranslationTask> tasks)
        {
            await _dbContext.TranslationTasks.AddRangeAsync(tasks);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TranslationTask> GetTranslationTaskById(long id, Func<IQueryable<TranslationTask>, IQueryable<TranslationTask>>? includeChain = null)
        {
            var query = _dbContext.TranslationTasks.AsQueryable();

            if (includeChain != null)
            {
                query = includeChain(query);
            }

            return await query.Where(d => d.Id == id && d.DeletedDate == null)
                              .FirstAsync();
        }

        public async Task UpdateTanslationTask(TranslationTask task)
        {
            task.ModifiedDate = DateTime.UtcNow;
            _dbContext.TranslationTasks.Update(task);
            await _dbContext.SaveChangesAsync();
        }
    }
}
