using SDT.Data.Models;

namespace SDT.Repositories
{
    public interface ITranslationTaskRepository
    {
        public Task<TranslationTask> GetTranslationTaskById(long id, Func<IQueryable<TranslationTask>, IQueryable<TranslationTask>>? includeChain = null);

        public Task AddedTanslationTasks(List<TranslationTask> tasks);

        public Task UpdateTanslationTask(TranslationTask task);
    }
}
