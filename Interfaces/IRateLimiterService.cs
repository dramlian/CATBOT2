
public interface IRateLimiterService
{
    bool IsAllowed(string userId, int maxRequests = 5, TimeSpan? window = null);
}
