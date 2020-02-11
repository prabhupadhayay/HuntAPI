using DiscoveryHuntApi.DAL;
using DiscoveryHuntApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DiscoveryHuntApi.Controllers
{
    public class ValuesController : ApiController
    {
       private DiscoveryHunt_DBEntities discoveryHunt_DBEntities = new DiscoveryHunt_DBEntities();
        // GET api/values
        public IEnumerable<string> Get()
        {
            try
            {
                var sss = discoveryHunt_DBEntities.Questions;
                return new string[] { "value1", "value2" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

    }
}



