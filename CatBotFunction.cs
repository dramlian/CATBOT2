using System;
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
    public void Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}