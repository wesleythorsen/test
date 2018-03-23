using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Timers;

// REST API
using Tweetinvi;
using Tweetinvi.Models;

// STREAM API
using Tweetinvi.Streaming;
using Stream = Tweetinvi.Stream;

namespace TweetStreamService
{
    public class TweetStreamer
    {
        private twitterEntities _Context;
        private BufferBlock<ITweet> _TargetBuffer;
        private ISampleStream _Stream;
        private Timer _NewTweetTimer;

        // Keys from twitter, required for using their api:
        private string _ConsumerKey = "gpBJE0rPOCF5Kn1zZkEB8unkb";
        private string _ConsumerSecret = "XDSqGsA9IQbVhtFS7Fu10F9np1kwHd3RTXtODOclDh08c5PMrr";
        private string _AccessToken = "563989979-R55H3UJdxEw5807o0xiHhOHKFgxbRWwsC4ffliHu";
        private string _AccessTokenSecret = "w6FPLXWmkIu22oC4J4J2rGZAograjUg151XsPKqjDNNuU";

        public void Start()
        {
            Library.WriteErrorLog("Tweet streamer starting.");

            Auth.SetUserCredentials(_ConsumerKey, _ConsumerSecret, _AccessToken, _AccessTokenSecret);

            _Context = new twitterEntities();
            _TargetBuffer = new BufferBlock<ITweet>();

            _NewTweetTimer = new Timer(10000);
            _NewTweetTimer.AutoReset = false;
            _NewTweetTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                Library.WriteErrorLog("No Tweets in 10 seconds. Reseting streamer.");
                this.Stop();
            };
            _NewTweetTimer.Start();

            // Start the consumer. The Consume method runs asynchronously. 
            var consumer = ConsumeAsync();

            Produce();

            consumer.Wait();
        }

        public void Stop()
        {
            _Context?.Dispose();

            // Set the target to the completed state to signal to the consumer that no more data will be available.
            _TargetBuffer?.Complete();

            _Stream?.StopStream();
            _Stream = null;

            Library.WriteErrorLog("Tweet streamer stoped (.Stop()).");
        }

        ~TweetStreamer()
        {
            _Context?.Dispose();

            // Set the target to the completed state to signal to the consumer that no more data will be available.
            _TargetBuffer?.Complete();

            _Stream?.StopStream();
            _Stream = null;

            Library.WriteErrorLog("Tweet streamer stoped (finalizer).");
        }

        private void Produce()
        {
            _Stream = Stream.CreateSampleStream();

            _Stream.TweetReceived += (object sender, Tweetinvi.Events.TweetReceivedEventArgs e) =>
            {
                _TargetBuffer.Post(e.Tweet);
            };

            _Stream.AddTweetLanguageFilter(LanguageFilter.English);

            _Stream.StartStreamAsync();
        }

        private async Task<int> ConsumeAsync()
        {
            int batchCounter = 0;
            var recentIds = new HashQueueCache<long>(1000);

            // Read from the source buffer until the source buffer has no available output data.
            while (await _TargetBuffer.OutputAvailableAsync())
            {
                ITweet tweet = _TargetBuffer.Receive();
                
                if (recentIds.Add(tweet.Id))
                {
                    _NewTweetTimer.Stop();
                    _NewTweetTimer.Start();

                    var tweetAdded = AddTweet(tweet);

                    if (tweetAdded) batchCounter++;
                    if (batchCounter >= 100)
                    {
                        // save in batches of 100
                        batchCounter = 0;
                        SaveContextBatch();

                        if (_TargetBuffer.Count > 1000) Library.WriteErrorLog("Warning: large buffer count: " + _TargetBuffer.Count);
                    }
                }
            }

            return 0;
        }
        
        private bool SaveContextBatch()
        {
            try
            {
                _Context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Library.WriteErrorLog("Entity validation error: " + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage);
                _Context?.Dispose();
                _Context = new twitterEntities();
                return false;
            }
            catch (Exception ex)
            {
                Library.WriteErrorLog(ex.GetInnermostException());
                _Context?.Dispose();
                _Context = new twitterEntities();
                return false;
            }
            return true;
        }

        private bool AddTweet(ITweet tivTweet)
        {
            string text = tivTweet.FullText;

            // If tweet is a retweet, store the original tweet's text.
            if (tivTweet.IsRetweet)
            {
                text = tivTweet.RetweetedTweet.FullText;
            }

            tweet sqlTweet = new tweet
            {
                id = tivTweet.Id,
                date = tivTweet.TweetLocalCreationDate,
                retweet = Convert.ToSByte(tivTweet.IsRetweet),
                text = text
            };

            _Context.tweets.Add(sqlTweet);

            return true;
        }
    }
}
