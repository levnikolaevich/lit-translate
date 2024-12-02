using SDT.Data;

namespace SDT.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly SDTContext _dbContext;

        protected BaseRepository(SDTContext context)
        {
            _dbContext = context;
        }
    }
}