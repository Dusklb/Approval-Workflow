using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Trinhky.Blazor.Server.Services
{

    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        //private readonly string _apiKey = "YOUR_GEMINI_API_KEY"; // Nên lưu trong appsettings.json
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"]; // Đọc từ config
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            //var requestUrl = $"{BaseUrl}?key={_apiKey}";

            // 1. CHỌN MODEL: Hãy thử đổi giữa 'gemini-1.5-flash' và 'gemini-pro' nếu vẫn lỗi
            string modelId = "gemini-2.5-flash";
            // string modelId = "gemini-pro"; // Dùng dòng này nếu flash bị lỗi 404

            // 2. URL CHUẨN: Lưu ý dấu hai chấm (:) trước generateContent
            string requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:generateContent?key={_apiKey}";
            var payload = new
            {
                contents = new[]
                {
                new { parts = new[] { new { text = prompt } } }
            }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                // Đọc lỗi chi tiết từ Google trả về thay vì chỉ nhìn thấy 404
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini Error {response.StatusCode}: {errorContent}");
            }
            //response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Parse JSON để lấy text (Cấu trúc trả về của Gemini)
            using var doc = JsonDocument.Parse(responseString);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text;
        }
    }
}
