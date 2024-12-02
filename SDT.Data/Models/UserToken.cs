using SDT.Data.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDT.Data.Models
{
    public class UserToken : BaseEntity<int>
    {
        public required string Token { get; set; }

        [ForeignKey(nameof(User))]
        public required long UserId { get; set; }

        public required virtual User User { get; set; }

        public required DateTime ExpirationDate { get; set; }
    }
}
