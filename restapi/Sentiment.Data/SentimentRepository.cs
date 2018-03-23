using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentiment.Common;
using Sentiment.Web.Api.Models;
using Sentiment.Analyzer;

namespace Sentiment.Data
{
    public class SentimentRepository : ISentimentRepository
    {
        private ITweetRepository _TweetRepo;
        private ISentimentAnalyzer _SentimentAnalyzer;
        private IConfigRepository _ConfigRepository;

        public SentimentRepository(ITweetRepository tweetRepo, ISentimentAnalyzer sentimentAnalyzer, IConfigRepository configRepository)
        {
            _TweetRepo = tweetRepo;
            _SentimentAnalyzer = sentimentAnalyzer;
            _ConfigRepository = configRepository;
        }

        public SentimentSeries GetSentimentByConfigId(long configId, TimeLibrary.TimeInterval groupingInterval, DateTime startDate, DateTime endDate)
        {
            var tweets = _TweetRepo.GetTweetsByConfigId(configId, startDate, endDate);

            var sentiment = _SentimentAnalyzer.GetSentiment(tweets, groupingInterval);

            var config = _ConfigRepository.GetConfigurationById(configId);

            var sentimentSeries = new SentimentSeries()
            {
                Keywords = new[] { config.Text },
                Sentiment = sentiment,
            };

            return sentimentSeries;
        }

        public SentimentSeries GetSentimentByKeyword(string keyword, TimeLibrary.TimeInterval groupingInterval, DateTime startDate, DateTime endDate)
        {
            var tweets = _TweetRepo.GetTweetsByKeyword(keyword, startDate, endDate);
            
            var sentiment = _SentimentAnalyzer.GetSentiment(tweets, groupingInterval);
            
            var sentimentSeries = new SentimentSeries()
            {
                Keywords = new[] { keyword },
                Sentiment = sentiment,
            };

            return sentimentSeries;
        }
    }
}
