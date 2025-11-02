public class MetaService : IMetaService
{
    private IHttpService _httpService;
    private const string _metaUrl = "https://graph.facebook.com/v21.0/me/messages";

    public MetaService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task SendImagePayloadMessage(string recipientId, string imageUrl, string PAT)
    {
        SendImageDto payload = new SendImageDto(recipientId,
            imageUrl, PAT);
        await _httpService.PostRequest<SendImageDto>(_metaUrl, payload);
    }
}