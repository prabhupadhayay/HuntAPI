using DiscoveryHuntApi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DiscoveryHuntApi.Controllers
{
    public class TrailController : ApiController
    {
        DiscoveryHunt_DBEntities discoveryHunt_DBEntities = new DiscoveryHunt_DBEntities();
        [System.Web.Http.HttpGet]
        [Route("GetTrails")]
        public IEnumerable<Models.Trail> Get()
        {
            try
            {
                List<Models.Trail> trailList = new List<Models.Trail>();
                var TrailData = discoveryHunt_DBEntities.Trials;
                foreach (var data in TrailData)
                {
                    Models.Trail trail = new Models.Trail();
                    trail.City = data.City;
                    trail.Country = data.Country;
                    trail.CreatedDate = DateTime.UtcNow;
                    trail.TrailId = data.TrialId;
                    trailList.Add(trail);

                }
                return trailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
