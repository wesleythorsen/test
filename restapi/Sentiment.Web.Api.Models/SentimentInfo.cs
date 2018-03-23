using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Web.Api.Models
{
    public class SentimentInfo
    {
        public double Sentiment { get; set; }
        public int Weight { get; set; }
    }
}
