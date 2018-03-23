using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sentiment.Data;
using Sentiment.Web.Api.Models;

namespace Sentiment.Web.Api.Controllers
{
    public class ConfigController : ApiController
    {
        private IConfigRepository _ConfigRepository;

        public ConfigController(IConfigRepository configRepository)
        {
            _ConfigRepository = configRepository;
        }

        [Route("config/all")]
        public IHttpActionResult GetAll()
        {
            return Ok(_ConfigRepository.GetAllConfigurationInfo());
        }

        [Route("config/byId/{id}")]
        public IHttpActionResult GetById(long id)
        {
            return Ok(_ConfigRepository.GetConfigurationById(id));
        }

        [Route("config/byUser/{user}")]
        public IHttpActionResult GetByUser(string user)
        {
            return Ok(_ConfigRepository.GetConfigurationByUser(user));
        }

        [Route("config/post"), HttpPost]
        public IHttpActionResult Post([FromBody] ConfigFileInfo cfgInfo)
        {
            var newCfg = _ConfigRepository.CreateNewConfig(cfgInfo.User, cfgInfo.Text);

            return Ok(newCfg);
        }
    }
}
