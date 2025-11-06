using System.Collections.Concurrent;

public class RateLimiterService : IRateLimiterService
{
    private readonly ConcurrentDictionary<string, Queue<DateTime>> _userRequests = new();
    private readonly object _lock = new();

    public bool IsAllowed(string userId, int maxRequests = 5, TimeSpan? window = null)
    {
        var timeWindow = window ?? TimeSpan.FromMinutes(1);
        var now = DateTime.UtcNow;

        lock (_lock)
        {
            if (!_userRequests.TryGetValue(userId, out var requests))
            {
                requests = new Queue<DateTime>();
                _userRequests[userId] = requests;
            }

            while (requests.Count > 0 && now - requests.Peek() > timeWindow)
            {
                requests.Dequeue();
            }

            if (requests.Count >= maxRequests)
            {
                return false;
            }

            requests.Enqueue(now);
            return true;
        }
    }
}
