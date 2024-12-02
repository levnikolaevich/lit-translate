using OpenAI.Chat;

namespace SDT.LBl.IServices
{
    public interface IOpenAIService
    {
        public Task<ChatCompletion> GetAnswer(string apiKey, string model, string prompt, string text);
    }
}
