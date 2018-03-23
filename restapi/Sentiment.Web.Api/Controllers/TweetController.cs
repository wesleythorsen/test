using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sentiment.Data;

namespace Sentiment.Web.Api.Controllers
{
    public class TweetController : ApiController
    {
        private ITweetRepository _TweetRepo;

        public TweetController(ITweetRepository tweetRepo)
        { 
            _TweetRepo = tweetRepo;
        }

        [Route("tweets/byId/{id}")]
        public IHttpActionResult GetById(long id)
        {
            var tweet = _TweetRepo.GetTweetById(id);

            return Ok(tweet);
        }

        [Route("tweets/byCfg/{configId}/{startDate?}/{endDate?}")]
        public IHttpActionResult GetByConfig(long configId, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.Now.ToUniversalTime();

            var tweets = _TweetRepo.GetTweetsByConfigId(configId, startDate.Value, endDate.Value, 100);
            
            return Ok(tweets);
        }

        [Route("tweets/byKeyword/{keyword}/{startDate?}/{endDate?}")]
        public IHttpActionResult GetByKeyword(string keyword, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.Now.ToUniversalTime();

            var tweets = _TweetRepo.GetTweetsByKeyword(keyword, startDate.Value, endDate.Value, 100);

            return Ok(tweets);
        }
    }
}
