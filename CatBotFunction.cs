using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CATBOT.Function;

public class CatBotFunction
{
    private readonly ILogger _logger;

    public CatBotFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CatBotFunction>();
    }

    [Function("CatBotFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);

        var PAT = Environment.GetEnvironmentVariable("META_PAT");
        var recipientID = Environment.GetEnvironmentVariable("RECIPIENT_ID");

        if (string.IsNullOrEmpty(PAT))
        {
            _logger.LogError("META_PAT environment variable is not set");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("META_PAT environment variable is not set");
            return response;
        }

        if (string.IsNullOrEmpty(recipientID))
        {
            _logger.LogError("RECIPIENT_ID environment variable is not set");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("RECIPIENT_ID environment variable is not set");
            return response;
        }

        ICatImageSender catImageSender = new CatImageSender();
        await catImageSender.SendCatImage(PAT, recipientID);

        await response.WriteStringAsync("Cat image sent successfully!");
        return response;
    }
}