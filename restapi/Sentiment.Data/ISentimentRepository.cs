using System;
using Sentiment.Web.Api.Models;
using Sentiment.Common;

namespace Sentiment.Data
{
    public interface ISentimentRepository
    {
        SentimentSeries GetSentimentByKeyword(string keyword, TimeLibrary.TimeInterval groupingInterval, DateTime startDate, DateTime endDate);
        SentimentSeries GetSentimentByConfigId(long configId, TimeLibrary.TimeInterval groupingInterval, DateTime startDate, DateTime endDate);
    }
}