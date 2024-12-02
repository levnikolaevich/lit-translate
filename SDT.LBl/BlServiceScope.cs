using Microsoft.Extensions.DependencyInjection;
using SDT.LBl.IServices;
using SDT.LBl.Services;

namespace SDT.LBl
{
    public static class BlServiceScope
    {
        public static IServiceCollection AddBlServices(this IServiceCollection services)
        {
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<IOpenAIService, OpenAIService>();
            services.AddScoped<IFileService, FileService>();

            services.AddSingleton<ChatClientFactory>();

            return services;
        }
    }
}
