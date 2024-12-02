using SDT.Data.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDT.Data.Models
{
    public class AILanguageModel : BaseEntity<int>
    {
        [ForeignKey(nameof(AICompany))]
        public required int AICompanyId { get; set; }
        public virtual AICompany? AICompany { get; set; }

        public required string Name { get; set; }

        public required float InputTokenPrice { get; set; } = 0;
        public required float OutputTokenPrice { get; set; } = 0;
        public required float CachedTokenPrice { get; set; } = 0;
    }
}
