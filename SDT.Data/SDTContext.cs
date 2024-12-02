using SDT.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace SDT.Data
{
    public class SDTContext(DbContextOptions<SDTContext> options) : DbContext(options)
    {
        // User's information
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserToken> UserTokens { get; set; }

        // AILanguageModel's information
        public virtual DbSet<AICompany> AICompanies { get; set; }
        public virtual DbSet<AILanguageModel> AILanguageModels { get; set; }
        public virtual DbSet<Language> Languages { get; set; }

        // TranslationTask's information
        public virtual DbSet<UserDocument> Documents { get; set; }
        public virtual DbSet<TranslationTask> TranslationTasks { get; set; }
        public virtual DbSet<OriginalParagraph> Paragraphs { get; set; }
        public virtual DbSet<ParagraphTranslation> ParagraphTranslations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи между Document и TranslationTask
            modelBuilder.Entity<UserDocument>()
                .HasMany(d => d.TranslationTasks) // Коллекция в Document
                .WithOne(t => t.Document) // Навигационное свойство в TranslationTask
                .HasForeignKey(t => t.DocumentId); // Внешний ключ в TranslationTask
        }
    }
}