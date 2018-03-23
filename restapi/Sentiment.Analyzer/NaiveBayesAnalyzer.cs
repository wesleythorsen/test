using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentiment.Web.Api.Models;
using Sentiment.Common;

namespace Sentiment.Analyzer
{
    public class NaiveBayesAnalyzer : ISentimentAnalyzer
    {
        public NaiveBayes _NaiveBayes;

        public NaiveBayesAnalyzer()
        {
            _NaiveBayes = new NaiveBayes(); // This constructor needs to initialize the save files
        }

        /// <summary>
        /// Gets the sentiment for a set of tweets. Uses the Naive Bayes method.
        /// </summary>
        /// <param name="tweets"></param>
        /// <returns></returns>
        public Dictionary<DateTime, SentimentInfo> GetSentiment(IEnumerable<Tweet> tweets, TimeLibrary.TimeInterval groupingInterval)
        {
            var sentimentScores = new Dictionary<DateTime, SentimentInfo>();

            //// This will group the tweets by hour
            //var tweetsGroupedByHour = tweets
            //    .GroupBy(t => new DateTime(t.CreatedAt.Year, t.CreatedAt.Month, t.CreatedAt.Day, t.CreatedAt.Hour, 0, 0));

            // This will group the tweets by month
            var tweetsGroupedByHour = tweets
                .GroupBy(t => new DateTime(t.CreatedAt.Year, t.CreatedAt.Month, 1, 0, 0, 0));

            // foreach group of tweets
            foreach (var tweetGroup in tweetsGroupedByHour)
            {
                var numberOfTweets = tweetGroup.Count();

                // calculate the sum of sentiment of each tweet in the group
                var sentiment = tweetGroup.Sum(t => CalculateSentiment(t)) / numberOfTweets;
                
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
        /// Calculates the sentiment of a tweet using the Naive Bayes method.
        /// </summary>
        /// <remarks>
        /// <param name="tweets"></param>
        /// <returns></returns>
        private double CalculateSentiment(Tweet tweet)
        {
            double sentiment = 0;

            bool result = _NaiveBayes.makePrediction(tweet.Text);

            if (result == true)
            {
                sentiment = 1;
            }
            else
            {
                sentiment = -1;
            }

            return sentiment;
        }
    }
}
