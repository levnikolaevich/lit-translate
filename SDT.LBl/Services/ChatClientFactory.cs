using OpenAI.Chat;

namespace SDT.LBl.Services
{
    public class ChatClientFactory
    {
        public ChatClient CreateClient(string apiKey, string model)
        {
            return new ChatClient(model: model, apiKey: apiKey);
        }
    }
}
