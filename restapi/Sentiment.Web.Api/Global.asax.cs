using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Lifestyles;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Integration.Web;
using Sentiment.Data;
using Sentiment.Data.MySql;
using Sentiment.Analyzer;

using System.Reflection;
using System.Web.Compilation;
using System.Web.UI;

namespace Sentiment.Web.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Create the container
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Register types
            container.Register<ITweetRepository, TweetRepository>(Lifestyle.Scoped);
            container.Register<ISentimentRepository, SentimentRepository>(Lifestyle.Scoped);
            container.Register<IConfigRepository, ConfigRepository>(Lifestyle.Scoped);

            container.Register<ITwitterEntities, TwitterEntities>(Lifestyle.Scoped);

            //container.Register<ISentimentAnalyzer, SimpleAnalyzer>(Lifestyle.Singleton);
            //container.Register<ISentimentAnalyzer, NaiveBayesAnalyzer>(Lifestyle.Singleton);
            container.Register<ISentimentAnalyzer, NaiveBayesAndFT>(Lifestyle.Singleton);

            // This is an extension method from the integration package.
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
            
            RegisterRoutes(RouteTable.Routes);
        }

        void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("Default",
                "",
                "~/Default.aspx");

            routes.MapPageRoute("ConfigUpload",
                "config/upload",
                "~/ConfigUpload.aspx");
        }
    }
}
