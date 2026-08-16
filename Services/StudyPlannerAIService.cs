using System.Text;
using System.Text.Json;

namespace AIStudyPlanner.Services
{
    public class StudyPlannerAIService
    {
        private readonly HttpClient _httpClient;

        public StudyPlannerAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateStudyPlanAsync(object plannerData)
        {
            var json = JsonSerializer.Serialize(plannerData);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "http://127.0.0.1:5050/generate",
                content);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Python AI Engine error: {response.StatusCode} - {responseContent}");
            }

            using var document =
                JsonDocument.Parse(responseContent);

            var root = document.RootElement;

            if (!root.TryGetProperty("success", out var success) ||
                !success.GetBoolean())
            {
                var error = root.TryGetProperty("error", out var errorElement)
                    ? errorElement.GetString()
                    : "Unknown Python AI Engine error.";

                throw new Exception(
                    $"Python AI Engine error: {error}");
            }

            return JsonSerializer.Serialize(
                root,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
    }
}