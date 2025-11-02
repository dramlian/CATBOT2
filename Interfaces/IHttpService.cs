public interface IHttpService
{
    public Task PostRequest<T>(string url, T payload);
    public Task<T?> GetRequest<T>(string url);

}