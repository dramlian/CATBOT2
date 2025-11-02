public interface IMetaService
{
    public Task SendImagePayloadMessage(string recipientId, string imageUrl, string PAT);
}