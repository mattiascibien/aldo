using Aldo.Config;
using Microsoft.Extensions.Options;
using Tweetinvi.Exceptions;

namespace Aldo
{
    public class BotWorker : BackgroundService
    {
        private readonly BotWorkerConfig _config;

        private readonly ILogger<BotWorker> _logger;

        public BotWorker(ILogger<BotWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration.GetRequiredSection(nameof(BotWorker)).Get<BotWorkerConfig>();
        }

        private void TryRetweet()
        {
            
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    TryRetweet();

                    await Task.Delay(_config.SleepTime, stoppingToken).ConfigureAwait(false);
                }
                catch (TwitterException twe)
                {

                    if(twe.StatusCode == 429)
                    {
                        _logger.LogWarning("Twitter rate limit reached. Sleeping for some time");
                        await Task.Delay(_config.RateLimitSleepTime, stoppingToken).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}