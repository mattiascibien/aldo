using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldo.Config
{
    public class BotWorkerConfig
    {
        public TimeSpan SleepTime { get; set; }

        public TimeSpan RateLimitSleepTime { get; set; }

        public string SearchQuery { get; set; } = string.Empty;

        public int TweetLimit { get; set; }
    }
}
