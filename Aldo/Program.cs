using Aldo.Config;
using Tweetinvi;

namespace Aldo
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            dotenv.net.DotEnv.Load();
#endif

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => config.AddEnvironmentVariables("ALDO_"))
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient((provider) =>
                    {
                        var twitterConfig = context.Configuration.GetRequiredSection(nameof(TwitterClient)).Get<TwitterClientConfig>();

                        TwitterClient client = new(
                            twitterConfig.ConsumerKey,
                            twitterConfig.ConsumerSecret,
                            twitterConfig.AccessToken,
                            twitterConfig.AccessSecret);

                        return client;
                    });
                    services.AddHostedService<BotWorker>();
                })
                .Build();

            host.Run();
        }
    }
}