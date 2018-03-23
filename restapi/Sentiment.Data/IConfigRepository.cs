using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentiment.Web.Api.Models;

namespace Sentiment.Data
{
    public interface IConfigRepository
    {
        ConfigFileInfo[] GetAllConfigurationInfo();
        ConfigFileInfo GetConfigurationById(long id);
        ConfigFileInfo[] GetConfigurationByUser(string user);
        ConfigFileInfo CreateNewConfig(string user, string configText);
    }
}
