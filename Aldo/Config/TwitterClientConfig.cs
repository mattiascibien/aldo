using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldo.Config
{
    public class TwitterClientConfig
    {
        public string ConsumerKey { get; set; } = string.Empty;

        public string ConsumerSecret { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public string AccessSecret { get; set; } = string.Empty;
    }
}
