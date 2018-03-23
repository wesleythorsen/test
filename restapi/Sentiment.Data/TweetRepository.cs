using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sentiment.Common;
using Sentiment.Data.MySql;
using Sentiment.Web.Api.Models;
using LinqKit;

namespace Sentiment.Data
{
    public class TweetRepository : ITweetRepository
    {
        private ITwitterEntities _TwitterEntities;
        private IConfigRepository _ConfigRepository;

        public TweetRepository(ITwitterEntities twitterEntities, IConfigRepository configRepository)
        {
            _TwitterEntities = twitterEntities;
            _ConfigRepository = configRepository;
        }

        public Web.Api.Models.Tweet GetTweetById(long id)
        {
            var sqlTweet = _TwitterEntities.tweets.SingleOrDefault(t => t.id == id);
            var tweet = ToTweet(sqlTweet);

            return tweet;
        }

        public Web.Api.Models.Tweet[] GetTweetsByConfigId(long configId, DateTime startDate, DateTime endDate, int limit = int.MaxValue)
        {
            var config = _ConfigRepository.GetConfigurationById(configId);
            var cfgString = config.Text;

            var predicate = TweetPredicateBuilder.BuildPredicate(cfgString);
            
            var sqlTweets = _TwitterEntities.tweets
                .AsExpandable()
                .Where(t => t.date >= startDate)
                .Where(t => t.date < endDate)
                .Where(predicate)
                .Take(limit)
                .ToArray();

            var tweets = sqlTweets
                .Select(t => ToTweet(t))
                .ToArray();

            return tweets;
        }
        
        public Web.Api.Models.Tweet[] GetTweetsByKeyword(string keyword, DateTime startDate, DateTime endDate, int limit = int.MaxValue)
        {
            var sqlTweets = _TwitterEntities.tweets
                .Where(t => t.date >= startDate)
                .Where(t => t.date < endDate)
                .Where(t => t.text.Contains(keyword))
                .Take(limit)
                .ToArray();

            var tweets = sqlTweets
                .Select(t => ToTweet(t))
                .ToArray();

            return tweets;
        }

        /// <summary>
        /// Converts SqlServer model(s) to data model.
        /// </summary>
        /// <param name="sqlTweet"></param>
        /// <returns></returns>
        private static Web.Api.Models.Tweet ToTweet(MySql.tweet sqlTweet)
        {
            if (sqlTweet == null) return null;

            return new Web.Api.Models.Tweet()
            {
                Id = sqlTweet.id,
                CreatedAt = sqlTweet.date,
                IsRetweet = Convert.ToBoolean(sqlTweet.retweet),
                Text = sqlTweet.text,
            };
        }
    }
}
