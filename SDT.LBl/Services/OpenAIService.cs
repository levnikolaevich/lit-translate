using OpenAI.Chat;
using SDT.LBl.IServices;

namespace SDT.LBl.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClientFactory _factory;

        public OpenAIService(ChatClientFactory factory) {
            _factory = factory;
        }

        public async Task<ChatCompletion> GetAnswer(string apiKey, string model, string prompt, string text)
        {
            var chatClient = _factory.CreateClient(apiKey, model);

            // Формируем запрос к модели
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(prompt), // Устанавливаем системный контекст
                new UserChatMessage(text) // Передаем текст пользователя                
            };

            // Отправляем запрос на обработку модели
            return await chatClient.CompleteChatAsync(messages);
        }
    }
}
