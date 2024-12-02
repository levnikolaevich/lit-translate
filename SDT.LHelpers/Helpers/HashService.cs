using SDT.Bl.IHelpers;
using System.Security.Cryptography;
using System.Text;

namespace SDT.Bl.Helpers
{
    public class HashService : IHashService
    {
        public string GenerateHash(string password, string salt)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(salt)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }

        public string GenerateSalt()
        {
            var randomBytes = new byte[32]; // Длина соли 32 байта
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}