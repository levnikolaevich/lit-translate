using SDT.Data.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDT.Data.Models
{
    public class ParagraphTranslation : BaseEntity<long>
    {
        [ForeignKey(nameof(Paragraph))]
        public long ParagraphId { get; set; }
        public virtual OriginalParagraph? Paragraph { get; set; }

        [ForeignKey(nameof(TranslationTask))]
        public required long TranslationTaskId { get; set; }
        public virtual TranslationTask? TranslationTask { get; set; }

        public required string TranslatedText { get; set; }

        public int? InputTokenCount { get; set; }
        public int? OutputTokenCount { get; set; }
        public int ProcessingTime { get; set; } = 0;
        public float? FinalPrice { get; set; }
    }
}
