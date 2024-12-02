using SDT.Data.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDT.Data.Models
{
    public class UserDocument : BaseEntity<long>
    {
        public required string Name { get; set; }
        public required string MimeType { get; set; }
        public required string FilePath { get; set; }

        [ForeignKey(nameof(Language))]
        public int? LanguageId { get; set; }
        public virtual Language? OriginalLanguageCode { get; set; }

        [ForeignKey(nameof(UserDocument))]
        public long? DocumentParentId { get; set; }
        public virtual UserDocument? DocumentParent { get; set; }

        [ForeignKey(nameof(User))]
        public long? UserId { get; set; }
        public virtual User? User { get; set; }

        public virtual ICollection<OriginalParagraph> Paragraphs { get; set; } = new List<OriginalParagraph>();

        public virtual ICollection<TranslationTask> TranslationTasks { get; set; } = new List<TranslationTask>();
    }
}
