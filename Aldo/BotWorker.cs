using Aldo.Config;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Core.Models;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Aldo
{
    public class BotWorker : BackgroundService
    {
        private readonly ILogger<BotWorker> _logger;
        private readonly BotWorkerConfig _config;
        private readonly TwitterClient _twitterClient;

        public BotWorker(ILogger<BotWorker> logger, IConfiguration configuration, TwitterClient twitterClient)
        {
            _twitterClient = twitterClient;
            _logger = logger;
            _config = configuration.GetRequiredSection(nameof(BotWorker)).Get<BotWorkerConfig>();
        }


        private async Task TryRetweetAsync(ITweet tweet)
        {
            var retweets = await _twitterClient.Tweets.GetRetweetsAsync(new GetRetweetsParameters()
            {
                Tweet = tweet
            });

            // if i have already retweeted this, do not retweet it
            if (retweets.Length > 0)
                return;

            _logger.LogInformation($"Tweeting {tweet.IdStr}");
            await _twitterClient.Tweets.PublishRetweetAsync(tweet.Id);
        }

        private async Task RetweetHashtagSearch()
        {
            foreach (var hashtag in _config.SearchQuery.Split(" "))
            {
                var tweetQuery = await _twitterClient.Search.SearchTweetsAsync($"{hashtag} -rt");
                var tweets = tweetQuery.Take(_config.TweetLimit);

                foreach (var tweet in tweets)
                { 
                    await TryRetweetAsync(tweet);
                }

            }          
        }

        private async Task RetweetMentions()
        {
            var mentionsTimeline = await _twitterClient.Timelines.GetMentionsTimelineAsync();
            var mentions = mentionsTimeline.Take(_config.TweetLimit);

            foreach (var mention in mentions)
            {
                await TryRetweetAsync(mention);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bot has started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RetweetHashtagSearch();

                    await RetweetMentions();

                    await Task.Delay(_config.SleepTime, stoppingToken).ConfigureAwait(false);
                }
                catch (TwitterException twe)
                {

                    if(twe.StatusCode == 429)
                    {
                        _logger.LogWarning("Twitter rate limit reached. Sleeping for some time");
                        await Task.Delay(_config.RateLimitSleepTime, stoppingToken).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Error, twe, "Exception in worker");
                    }
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Critical, e, "Exception in worker");
                }
            }
            _logger.LogInformation("Bot has stopped");
        }
    }
}