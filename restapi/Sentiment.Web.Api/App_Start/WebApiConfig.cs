using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Sentiment.Web.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings
                .Add(new System.Net.Http.Formatting.RequestHeaderMapping(
                    "Accept",
                    "text/html",
                    StringComparison.InvariantCultureIgnoreCase,
                    true,
                    "application/json"));
        }
    }
}
