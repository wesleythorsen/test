using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentiment.Web.Api.Models;
using System.Linq.Expressions;

namespace Sentiment.Data
{
    public interface ITweetRepository
    {
        Tweet GetTweetById(long id);
        Tweet[] GetTweetsByKeyword(string keyword, DateTime startDate, DateTime endDate, int limit = int.MaxValue);
        Tweet[] GetTweetsByConfigId(long configId, DateTime startDate, DateTime endDate, int limit = int.MaxValue);
    }
}
