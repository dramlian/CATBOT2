using System.Text;
using System.Text.Json;

public class HttpService : IHttpService
{
    private HttpClient _httpClient;
    public HttpService()
    {
        _httpClient = new HttpClient();
    }

    public async Task PostRequest<T>(string url, T payload)
    {
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var res = await _httpClient.PostAsync(url, content);
        res.EnsureSuccessStatusCode();
    }

    public async Task<T?> GetRequest<T>(string url)
    {
        var res = await _httpClient.GetAsync(url);
        res.EnsureSuccessStatusCode();

        var jsonVal = await res.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<T>(jsonVal, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data;
    }
}