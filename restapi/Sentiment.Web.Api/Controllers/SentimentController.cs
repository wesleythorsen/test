using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sentiment.Data;

namespace Sentiment.Web.Api.Controllers
{
    public class SentimentController : ApiController
    {
        private ISentimentRepository _SentimentRepo;

        public SentimentController(ISentimentRepository sentimentRepo)
        {
            _SentimentRepo = sentimentRepo;
        }

        [Route("sentiment/byKeyword/{keyword}/{startDate?}/{endDate?}/{period?}")]
        public IHttpActionResult GetSentiment(string keyword, DateTime? startDate = null, DateTime? endDate = null, string period = "d")
        {
            var groupingInterval = Common.TimeLibrary.StringToTimeInterval(period);

            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.Now.ToUniversalTime();

            var sentiment = _SentimentRepo.GetSentimentByKeyword(keyword, groupingInterval, startDate.Value, endDate.Value);

            return Ok(sentiment);
        }

        [Route("sentiment/byCfg/{configId}/{startDate?}/{endDate?}/{period?}")]
        public IHttpActionResult GetSentiment(long configId, DateTime? startDate = null, DateTime? endDate = null, string period = "d")
        {
            var groupingInterval = Common.TimeLibrary.StringToTimeInterval(period);

            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.Now.ToUniversalTime();

            var sentiment = _SentimentRepo.GetSentimentByConfigId(configId, groupingInterval, startDate.Value, endDate.Value);

            return Ok(sentiment);
        }
    }
}
