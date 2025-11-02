using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CATBOT.Function;

public class CatBotFunction
{
    private readonly ILogger _logger;

    public CatBotFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CatBotFunction>();
    }

    [Function("CatBotFunction")]
    public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer)
    {
        var PAT = Environment.GetEnvironmentVariable("META_PAT");
        var recipientID = Environment.GetEnvironmentVariable("RECIPIENT_ID");

        if (string.IsNullOrEmpty(PAT))
        {
            _logger.LogError("META_PAT environment variable is not set");
            return;
        }

        if (string.IsNullOrEmpty(recipientID))
        {
            _logger.LogError("RECIPIENT_ID environment variable is not set");
            return;
        }

        ICatImageSender catImageSender = new CatImageSender();
        await catImageSender.SendCatImage(PAT, recipientID);
    }
}