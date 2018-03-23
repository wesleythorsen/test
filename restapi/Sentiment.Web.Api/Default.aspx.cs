using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sentiment.Analyzer;

namespace Sentiment.Web.Api
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var analyzer = GlobalConfiguration.Configuration.DependencyResolver
                .GetService(typeof(ISentimentAnalyzer))
                .ToString();

            var analyzerLiteral = new Literal();
            analyzerLiteral.Text = $"<b>{analyzer}</b>";
            this.FindControl("analyzer").Controls.Add(analyzerLiteral);
        }
    }
}