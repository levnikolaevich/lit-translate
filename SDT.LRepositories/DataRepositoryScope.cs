using Microsoft.Extensions.DependencyInjection;

namespace SDT.Repositories
{
    public static class DataRepositoryScope
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ITranslationRepository, TranslationRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<ITranslationTaskRepository, TranslationTaskRepository>();
            return services;
        }
    }
}
