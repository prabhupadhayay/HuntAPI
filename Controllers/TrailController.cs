using DiscoveryHuntApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DiscoveryHuntApi.Models;

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
                    trail.TrailName = data.TrailName;
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

        [System.Web.Http.HttpPost]
        [Route("CreateTrail")]
        public async Task<HttpResponseMessage> CreateTrail(Trail trail)
        {
            using (var context = new DiscoveryHunt_DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            DAL.Trial trial = new DAL.Trial();
                           
                            trial.City = trail.City;
                            trial.Country = trail.Country;
                            trial.CreatedDate = DateTime.UtcNow;
                            trial.TrailName = trail.TrailName;
                            context.Trials.Add(trial);
                            context.SaveChanges();
                            transaction.Commit();
                            return Request.CreateResponse(HttpStatusCode.OK, "Trail Created Successfully");
                        }
                        var errors = new List<string>();
                        foreach (var state in ModelState)
                        {
                            foreach (var error in state.Value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        var errorList = ModelState.ToDictionary(kvp => kvp.Key.Replace("trail.", ""), kvp => kvp.Value.Errors[0].ErrorMessage);

                        return Request.CreateResponse(HttpStatusCode.OK, errorList);

                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong");
                    }
                }
            }
          
        }
        [System.Web.Http.HttpPut]
        [Route("UpdateTrail")]
        public async Task<HttpResponseMessage> UpdateTrail(Trail trail)
        {
            using (var context = new DiscoveryHunt_DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var get_trail = context.Trials.Where(a => a.TrialId == trail.TrailId).FirstOrDefault();
                            if(get_trail == null)
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Trail with id = " + trail.TrailId.ToString() + "not found");
                            }
                            DAL.Trial trial = context.Trials.Where(a => a.TrialId == trail.TrailId).FirstOrDefault();
                           
                            trial.City = trail.City;
                            trial.Country = trail.Country;
                            trial.CreatedDate = DateTime.UtcNow;
                            trial.TrailName = trail.TrailName;
                            context.Entry(trial).State = EntityState.Modified;
                            context.SaveChanges();
                            transaction.Commit();
                            return Request.CreateResponse(HttpStatusCode.OK, "Trail Updated Successfully");
                        }
                        var errors = new List<string>();
                        foreach (var state in ModelState)
                        {
                            foreach (var error in state.Value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        var errorList = ModelState.ToDictionary(kvp => kvp.Key.Replace("trail.", ""), kvp => kvp.Value.Errors[0].ErrorMessage);

                        return Request.CreateResponse(HttpStatusCode.BadRequest, errorList);

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong");
                    }
                }
            }

        }

    }
}
