using SDT.Data.BaseEntity;
using SDT.Data.Enums;

namespace SDT.Data.Models
{
    public class User : BaseEntity<long>
    {
        public required string Login { get; set; }

        public required string Password { get; set; }

        public required string Salt { get; set; }

        public required bool IsActive { get; set; } = true;

        public required UserRole Role { get; set; } = UserRole.Client;

        public virtual ICollection<UserToken> UserTokens { get; set; } = new HashSet<UserToken>();
    }
}
