using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Web.Api.Models
{
    public class Tweet
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRetweet { get; set; }
        public string Text { get; set; }
    }
}
