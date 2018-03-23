using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Sentiment.Common;
using Sentiment.Data;
//using Sentiment.Data.SqlServer;
using Sentiment.Data.MySql;

namespace Sentiment.Web.Api
{
    public partial class ConfigUpload : System.Web.UI.Page
    {
        private IConfigRepository _ConfigRepository = new ConfigRepository(new TwitterEntities());

        //public ConfigUpload(IConfigRepository configRepository)
        //{
        //    _ConfigRepository = configRepository;
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _Errors = new List<string>();
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            var file = FileInput.PostedFile;
            var sr = new StreamReader(file.InputStream);
            var fileText = sr.ReadToEnd();

            if (!ValidateFile(file, fileText)) return;
            
            var configRecord = _ConfigRepository.CreateNewConfig("admin", fileText);

            Response.Redirect($"~/config/byId/{configRecord.Id}");
        }

        private List<string> _Errors = new List<string>();

        private void ShowErrors()
        {
            var errorLiteral = new Literal();
            errorLiteral.Text = "<font size=\"3\" color=\"red\">";
            foreach (var error in _Errors)
            {
                errorLiteral.Text += error + "<br />";
            }
            errorLiteral.Text += "</font>";

            this.FindControl("messageArea").Controls.Add(errorLiteral);
        }

        private bool ValidateFile(HttpPostedFile file, string fileText)
        {
            if (Path.GetExtension(file.FileName) != ".cfg")
            {
                _Errors.Add("File must be a .cfg file.");
            }

            if (TweetPredicateBuilder.ValidatePredicateString(fileText) == false)
            {
                _Errors.Add("File content invalid.");
            }
            
            if (_Errors.Count != 0)
            {
                ShowErrors();
                return false;
            }
            return true;
        }
    }
}