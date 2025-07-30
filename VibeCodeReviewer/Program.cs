using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

await ExecuteCodeReview();

static async Task ExecuteCodeReview()
{
    string apiKey = "sk-or-v1-33c0e5bd-d984ac9e507f4c0b0689623111c7c6c0-da5c6209622c89e361a03c85"; // Replace with your actual key
    string model = "mistralai/mistral-7b-instruct"; // Or use another like "mistralai/mistral-7b-instruct"


    string code = "Console.WriteLine(\"Test\")";

    var payload = new
    {
        model,
        messages = new[]
        {
                new { role = "system", content = "You are a senior software engineer reviewing code." },
                new { role = "user", content = $"Please review this C# code and suggest improvements:\n\n{code}" }
            }
    };

    using var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
    var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

    if (response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseBody);
        string feedback = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        Console.WriteLine("\n🧠 AI Feedback:\n");
        Console.WriteLine(feedback);
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    Console.ReadLine();
}
