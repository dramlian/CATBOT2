using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CATBOT.Function;

public class CatBotFunction
{
    private readonly ILogger _logger;
    private readonly IRateLimiterService _rateLimiter;

    public CatBotFunction(ILoggerFactory loggerFactory, IRateLimiterService rateLimiter)
    {
        _logger = loggerFactory.CreateLogger<CatBotFunction>();
        _rateLimiter = rateLimiter;
    }

    [Function("CatBotFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        if (req.Method == "POST")
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"Received POST payload: {body}");

            if (!IsMetaRequestValid(body, req))
            {
                _logger.LogWarning("Invalid Meta signature");
                var forbidden = req.CreateResponse(HttpStatusCode.Forbidden);
                await forbidden.WriteStringAsync("Invalid signature");
                return forbidden;
            }

            var senderId = JsonDocument.Parse(body).RootElement
                .GetProperty("entry")[0]
                .GetProperty("messaging")[0]
                .GetProperty("sender")
                .GetProperty("id")
                .GetString() ?? throw new Exception("No sender ID was found!");

            if (!_rateLimiter.IsAllowed(senderId))
            {
                _logger.LogWarning($"Rate limit exceeded for user {senderId}");
                var rateLimited = req.CreateResponse(HttpStatusCode.TooManyRequests);
                await rateLimited.WriteStringAsync("Rate limit exceeded. Please try again later.");
                return rateLimited;
            }

            var PAT = Environment.GetEnvironmentVariable("META_PAT");

            if (string.IsNullOrEmpty(PAT))
            {
                _logger.LogError("META_PAT environment variable is not set");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync("META_PAT environment variable is not set");
                return error;
            }

            ICatImageSender catImageSender = new CatImageSender();
            await catImageSender.SendCatImage(PAT, senderId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Cat image sent successfully!");
            return response;
        }
        else
        {
            var responsee = req.CreateResponse(HttpStatusCode.NotFound);
            return responsee;
        }
    }

    private bool IsMetaRequestValid(string body, HttpRequestData req)
    {
        req.Headers.TryGetValues("X-Hub-Signature-256", out var sigValues);
        var signature = sigValues?.FirstOrDefault();

        var secret = Environment.GetEnvironmentVariable("META_APP_SECRET");
        if (string.IsNullOrEmpty(signature) || !signature.StartsWith("sha256="))
            return false;

        var provided = signature.Substring("sha256=".Length);
        if (string.IsNullOrEmpty(secret))
            return false;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();

        return computed == provided.ToLower();
    }

}