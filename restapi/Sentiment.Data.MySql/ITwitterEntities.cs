using System.Data.Entity;

namespace Sentiment.Data.MySql
{
    public interface ITwitterEntities
    {
        int SaveChanges();
        DbSet<tweet> tweets { get; set; }
        DbSet<configinfo> configinfoes { get; set; }
    }
}
