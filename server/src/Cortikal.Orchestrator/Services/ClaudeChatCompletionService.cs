using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Cortikal.Orchestrator.Services;

public class ClaudeChatCompletionService : IChatCompletionService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _apiKey;

    public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

    public ClaudeChatCompletionService(string apiKey, string model)
    {
        _apiKey = apiKey;
        _model = model;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
    }

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chatHistory, 
        PromptExecutionSettings? executionSettings = null, 
        Kernel? kernel = null, 
        CancellationToken cancellationToken = default)
    {
        var messages = new List<object>();
        string systemMessage = string.Empty;

        foreach (var msg in chatHistory)
        {
            if (msg.Role == AuthorRole.System)
            {
                systemMessage += msg.Content + "\n";
            }
            else if (msg.Role == AuthorRole.User)
            {
                messages.Add(new { role = "user", content = msg.Content });
            }
            else if (msg.Role == AuthorRole.Assistant)
            {
                messages.Add(new { role = "assistant", content = msg.Content });
            }
        }

        var requestBody = new
        {
            model = _model,
            max_tokens = 4096,
            system = systemMessage,
            messages = messages
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.anthropic.com/v1/messages", content, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
        
        var textContent = responseObj.GetProperty("content")[0].GetProperty("text").GetString() ?? string.Empty;
        
        return new List<ChatMessageContent> 
        { 
            new ChatMessageContent(AuthorRole.Assistant, textContent) 
        };
    }

    public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chatHistory, 
        PromptExecutionSettings? executionSettings = null, 
        Kernel? kernel = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var contents = await GetChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);
        foreach (var content in contents)
        {
            yield return new StreamingChatMessageContent(content.Role, content.Content);
        }
    }
}
