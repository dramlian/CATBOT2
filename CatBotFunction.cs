using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CATBOT.Function;

public class CatBotFunction
{
    private readonly ILogger _logger;

    public CatBotFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CatBotFunction>();
    }

    [Function("CatBotFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

        if (req.Method == "GET")
        {
            var mode = query["hub.mode"];
            var token = query["hub.verify_token"];
            var challenge = query["hub.challenge"];

            if (mode == "subscribe" && token == Environment.GetEnvironmentVariable("VERIFY_TOKEN") && !String.IsNullOrEmpty(challenge))
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync(challenge, Encoding.UTF8);
                return response;
            }

            return req.CreateResponse(HttpStatusCode.Forbidden);
        }
        else if (req.Method == "POST")
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"Received POST payload: {requestBody}");

            var senderId = JsonDocument.Parse(requestBody).RootElement
                .GetProperty("entry")[0]
                .GetProperty("messaging")[0]
                .GetProperty("sender")
                .GetProperty("id")
                .GetString() ?? throw new Exception("No sender ID was found!");

            _logger.LogInformation($"Sender ID: {senderId}");
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
}