using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sentiment.Data;
using System.Data;
using System.Data.Common;
using System.IO;

namespace Sentiment.Web.Api.Controllers
{
    public class SystemController : ApiController
    {
        //[Route("system/providers")]
        //public IHttpActionResult GetProviders()
        //{
        //    // Retrieve the installed providers and factories.
        //    DataTable table = DbProviderFactories.GetFactoryClasses();

        //    var vals = table.Rows
        //        .Cast<DataRow>()
        //        .Select(r => table.Columns
        //            .Cast<DataColumn>()
        //            .Select(c => r[c])
        //            .ToArray())
        //        .ToArray();

        //    return Ok(vals);
        //}

        [Route("system/ip")]
        public IHttpActionResult GetIP()
        {
            string externalip = new WebClient().DownloadString("http://icanhazip.com").Replace("\n", "");

            return Ok(externalip);
        }

        [Route("system/temppath")]
        public IHttpActionResult GetTempPath()
        {
            return Ok(Path.GetTempPath());
        }


        //[Route("system/files")]
        //public IHttpActionResult GetFiles()
        //{
        //    var sysDir = Environment.SystemDirectory;
        //    var rootDir = Path.GetPathRoot(sysDir);
        //    var rootDirs = Directory.EnumerateDirectories(rootDir);
        //    var rootFiles = Directory.EnumerateFiles(rootDir);

        //    var info = new
        //    {
        //        sysDir,
        //        rootDir,
        //        rootDirs,
        //        rootFiles
        //    };

        //    return Ok(info);
        //}
    }
}