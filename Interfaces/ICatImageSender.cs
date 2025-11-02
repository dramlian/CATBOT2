public interface ICatImageSender
{
    public Task SendCatImage(string PAT, string recipientId);
}