using SDT.Data.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDT.Data.Models
{
    public class OriginalParagraph : BaseEntity<long>
    {
        [ForeignKey(nameof(Document))]
        public long DocumentId { get; set; }
        public virtual UserDocument? Document { get; set; }
        public required string Text { get; set; }
        public required int CharacterCount { get; set; }
        public int? EstimatedTokenCount { get; set; }
        public float? EstimatedPrice { get; set; }

        public virtual ICollection<ParagraphTranslation> ParagraphTranslations { get; set; } = new List<ParagraphTranslation>();
    }
}
