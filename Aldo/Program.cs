using Aldo.Config;

namespace Aldo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<BotWorker>();
                })
                .Build();

            host.Run();
        }
    }
}