namespace SDT.Bl.IHelpers
{
    public interface IHashService
    {
        string GenerateHash(string password, string salt);
        string GenerateSalt();
    }
}
