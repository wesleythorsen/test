using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Web.Api.Models
{
    public class SentimentSeries
    {
        public string[] Keywords { get; set; }
        public Dictionary<DateTime, SentimentInfo> Sentiment { get; set; }
    }
}
