using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentiment.Common;
using Sentiment.Web.Api.Models;
using Tweet = Sentiment.Web.Api.Models.Tweet;


namespace Sentiment.Analyzer
{
    public class SimpleAnalyzer : ISentimentAnalyzer
    {
        public SimpleAnalyzer()
        {
            // Init neural networks here
        }

        /// <summary>
        /// Gets the sentiment for a set of tweets. THIS IS FOR DEMO PURPOSES!
        /// </summary>
        /// <param name="tweets"></param>
        /// <returns></returns>
        public Dictionary<DateTime, SentimentInfo> GetSentiment(IEnumerable<Tweet> tweets, TimeLibrary.TimeInterval groupingInterval)
        {
            // Start sentiment anaysis here
            // Should return a dictionary with a datetime and a sentiment value

            // EXAMPLE:

            var sentimentScores = new Dictionary<DateTime, SentimentInfo>();

            // This will group the tweets by hour
            var tweetsGroupedByHour = tweets
                .GroupBy(t => new DateTime(t.CreatedAt.Year, t.CreatedAt.Month, t.CreatedAt.Day, t.CreatedAt.Hour, 0, 0));

            // foreach group of tweets
            foreach (var tweetGroup in tweetsGroupedByHour)
            {
                // calculate the sum of sentiment of each tweet in the group
                var sentiment = tweetGroup.Sum(t => CalculateSentiment(t));

                var numberOfTweets = tweetGroup.Count();

                // add the datetime and the sentiment info to the sentiment dictionary
                sentimentScores
                    .Add(
                        tweetGroup.Key,
                        new SentimentInfo()
                        {
                            Sentiment = sentiment,
                            Weight = numberOfTweets
                        });
            }

            return sentimentScores;
        }

        /// <summary>
        /// Calculates the sentiment of a tweets by counting how many times it says "good" or "bad".
        /// </summary>
        /// <remarks>
        /// Obviously, this is a terrible way to calculate sentiment, it is only for demo purposes.
        /// </remarks>
        /// <param name="tweets"></param>
        /// <returns></returns>
        private double CalculateSentiment(Tweet tweet)
        {
            // count tweets that have the word "good" in it
            double goodSentiment = tweet.Text.CountOccurances("good");

            // count tweets that have the words bad in it
            double badSentiment = tweet.Text.CountOccurances("bad");

            // calculate the setiment by adding number of times good was encountered minus the number of times bad was encountered
            double sentiment = goodSentiment - badSentiment;

            return sentiment;
        }
    }
}
