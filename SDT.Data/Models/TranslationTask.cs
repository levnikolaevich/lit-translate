using SDT.Data.BaseEntity;
using SDT.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDT.Data.Models
{
    public class TranslationTask : BaseEntity<long>
    {
        [ForeignKey(nameof(Document))]
        public long DocumentId { get; set; }
        public virtual UserDocument? Document { get; set; }

        [ForeignKey(nameof(TranslatedDocument))]
        public long? TranslatedDocumentId { get; set; }
        public virtual UserDocument? TranslatedDocument { get; set; }

        public TranslationTaskStatus Status { get; set; } = TranslationTaskStatus.New;

        public required string Prompt { get; set; }

        [ForeignKey(nameof(Language))]
        public int? LanguageId { get; set; }
        public virtual Language? LanguageCode { get; set; }

        [ForeignKey(nameof(AILanguageModel))]
        public required int AILanguageModelId { get; set; }
        public virtual AILanguageModel? AILanguageModel { get; set; }

        public required string ApiKey { get; set; }

        public int? EstimatedTokenCount { get; set; }
        public int? FinalTokenCount { get; set; }

        public float? EstimatedPrice { get; set; }
        public float? FinalPrice { get; set; }
        
        public required float Progress { get; set; }

        public string? ErrorDescription { get; set; }

        public int ProcessingTime { get; set; }

        public virtual ICollection<ParagraphTranslation> ParagraphTranslations { get; set; } = new List<ParagraphTranslation>();
    }
}
