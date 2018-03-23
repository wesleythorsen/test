using System;
using System.Collections.Generic;
using Sentiment.Web.Api.Models;
using Sentiment.Common;

namespace Sentiment.Analyzer
{
    public interface ISentimentAnalyzer
    {
        Dictionary<DateTime, SentimentInfo> GetSentiment(IEnumerable<Tweet> tweets, TimeLibrary.TimeInterval groupingInterval);
    }
}