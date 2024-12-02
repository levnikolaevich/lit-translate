using SDT.Data.Models;

namespace SDT.Repositories
{
    public interface IDocumentRepository
    {
        public Task<UserDocument> GetDocumentById(long id, Func<IQueryable<UserDocument>, IQueryable<UserDocument>>? includeChain = null);
        public Task<IEnumerable<UserDocument>> GetDocumentByIds(List<long> ids);
        public Task<IEnumerable<UserDocument>> GetDocumentByUserId(long userId);
        public Task AddDocument(UserDocument document);
        public Task<UserDocument> UpdateDocument(UserDocument document);
        public Task<UserDocument> DeleteDocument(int id);
    }
}
