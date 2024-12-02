using Microsoft.EntityFrameworkCore;
using SDT.Data;
using SDT.Data.Models;

namespace SDT.Repositories
{
    public class DocumentRepository(SDTContext context) : BaseRepository(context), IDocumentRepository
    {
        public async Task AddDocument(UserDocument document)
        {
            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();
        }

        public Task<UserDocument> DeleteDocument(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDocument> GetDocumentById(long id, Func<IQueryable<UserDocument>, IQueryable<UserDocument>>? includeChain = null)
        {
            var query = _dbContext.Documents.AsQueryable();

            if (includeChain != null)
            {
                query = includeChain(query);
            }

            return await query.Where(d => d.Id == id && d.DeletedDate == null)
                              .FirstAsync();
        }

        public Task<IEnumerable<UserDocument>> GetDocumentByIds(List<long> ids)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDocument>> GetDocumentByUserId(long userId)
        {
            return await _dbContext.Documents
                            .Where(d => d.UserId == userId && d.DeletedDate == null)
                            .ToListAsync();
        }

        public Task<UserDocument> UpdateDocument(UserDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
