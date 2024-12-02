using SDT.Data.BaseEntity;

namespace SDT.Data.Models
{
    public class Language : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}
