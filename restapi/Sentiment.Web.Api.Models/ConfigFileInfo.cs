using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Web.Api.Models
{
    public class ConfigFileInfo
    {
        public long Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
    }
}
