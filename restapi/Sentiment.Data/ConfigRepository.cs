using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentiment.Data.MySql;
using Sentiment.Web.Api.Models;

namespace Sentiment.Data
{
    public class ConfigRepository : IConfigRepository
    {
        private ITwitterEntities _TwitterEntities;
        
        public ConfigRepository(ITwitterEntities twitterEntities)
        {
            _TwitterEntities = twitterEntities;
        }

        public ConfigFileInfo CreateNewConfig(string userName, string configText)
        {
            var configRecord = new configinfo()
            {
                text = configText,
                user = userName
            };
            
            configRecord = _TwitterEntities.configinfoes.Add(configRecord);

            _TwitterEntities.SaveChanges();
            
            return new ConfigFileInfo
            {
                Id = configRecord.id,
                Text = configRecord.text,
                User = configRecord.user
            };
        }

        public ConfigFileInfo[] GetAllConfigurationInfo()
        {
            return _TwitterEntities.configinfoes
                .Select(c => new ConfigFileInfo()
                {
                    Id = c.id,
                    User = c.user,
                    Text = c.text
                })
                .ToArray();
        }

        public ConfigFileInfo GetConfigurationById(long id)
        {
            var cfg = _TwitterEntities.configinfoes
                .SingleOrDefault(c => c.id == id);

            return new ConfigFileInfo()
            {
                Id = cfg.id,
                User = cfg.user,
                Text = cfg.text
            };
        }

        public ConfigFileInfo[] GetConfigurationByUser(string user)
        {
            return _TwitterEntities.configinfoes
                .Where(c => c.user == user)
                .Select(c => new ConfigFileInfo()
                {
                    Id = c.id,
                    User = c.user,
                    Text = c.text
                })
                .ToArray();
        }

        //public ConfigFileInfo CreateNewConfig(string userName, string configText)
        //{
        //    var configRecord = _TwitterEntities.configinfoes.Add(new configinfo());

        //    _TwitterEntities.SaveChanges();

        //    var key = $"{configRecord.id}.cfg";

        //    var savePath = $@"C:\Temp\{key}";
        //    File.WriteAllText(savePath, configText);

        //    configRecord.key = key;
        //    _TwitterEntities.SaveChanges();

        //    // save to aws s3:
        //    TransferUtility fileTransferUtility = new
        //        TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USWest2));

        //    fileTransferUtility.Upload(savePath, "sentiment-config-storage", key);

        //    return new ConfigFileInfo
        //    {
        //        Id = configRecord.id,
        //        Text = configText
        //    };
        //}

        //public ConfigFileInfo[] GetAllConfigurationInfo()
        //{
        //    return _TwitterEntities.configinfoes
        //        .ToArray()
        //        .Select(c => new ConfigFileInfo
        //        {
        //            Id = c.id,
        //            Text = GetConfigFromS3(c.key)
        //        })
        //        .ToArray();
        //}

        //public ConfigFileInfo[] GetConfigurationByUser(string user)
        //{
        //    return _TwitterEntities.configinfoes
        //        .Where(c => c.user == user)
        //        .ToArray()
        //        .Select(c => new ConfigFileInfo
        //        {
        //            Id = c.id,
        //            User = c.user,
        //            Text = GetConfigFromS3(c.path)
        //        })
        //        .ToArray();
        //}

        //public ConfigFileInfo GetConfigurationById(long id)
        //{
        //    var config = _TwitterEntities.configinfoes
        //        .SingleOrDefault(f => f.id == id);

        //    if (config == null) return null;

        //    return new ConfigFileInfo
        //    {
        //        Id = config.id,
        //        Text = GetConfigFromS3(config.key)
        //    };
        //}

        //private string GetConfigFromS3(string key)
        //{
        //    // save to aws s3:
        //    TransferUtility fileTransferUtility = new
        //        TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USWest2));

        //    fileTransferUtility.Download($@"C:\Temp\{key}", "sentiment-config-storage", key);

        //    return File.ReadAllText($@"C:\Temp\{key}");
        //}
    }
}
