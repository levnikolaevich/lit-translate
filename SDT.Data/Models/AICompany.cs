using SDT.Data.BaseEntity;

namespace SDT.Data.Models
{
    public class AICompany : BaseEntity<int>
    {
        public required string Name { get; set; }
    }
}
