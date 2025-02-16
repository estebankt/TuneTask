using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using TuneTask.Core.Interfaces;

namespace TuneTask.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiApiKey;
    private const string OpenAiEmbeddingsEndpoint = "https://api.openai.com/v1/embeddings";
    private const string EmbeddingModel = "text-embedding-3-small";
    private const int DefaultEmbeddingSize = 1536;

    public AIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _openAiApiKey = configuration["OpenAI:ApiKey"]!;
    }

    public async Task<float[]> GenerateTaskEmbeddingAsync(string taskDescription)
    {
        var requestBody = new
        {
            input = taskDescription,
            model = EmbeddingModel
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);
        var response = await _httpClient.PostAsync(OpenAiEmbeddingsEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI API Error: {errorResponse}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        
        var jsonResult = JsonSerializer.Deserialize<OpenAiEmbeddingResponse>(
            jsonResponse, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
         );

        if (jsonResult?.Data == null || jsonResult.Data.Count == 0)
        {
            throw new Exception("OpenAI API returned an empty embedding response.");
        }

        return jsonResult.Data.First().Embedding.ToArray();
    }
}

// Helper Models

public class OpenAiEmbeddingResponse
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("data")]
    public List<OpenAiEmbeddingData> Data { get; set; } = new();

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("usage")]
    public OpenAiUsage Usage { get; set; }
}

public class OpenAiEmbeddingData
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("embedding")]
    public List<float> Embedding { get; set; } = new();
}

public class OpenAiUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
