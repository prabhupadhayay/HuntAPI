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
    public class UserProfileController : ApiController
    {
        DiscoveryHunt_DBEntities discoveryHunt_DBEntities = new DiscoveryHunt_DBEntities();
        [System.Web.Http.HttpGet]
        [Route("GetUsers")]
        public IEnumerable<Models.UserProfile> Get()
        {
            try
            {
                List<Models.UserProfile> userProfiileList = new List<Models.UserProfile>();
                var UsersData = discoveryHunt_DBEntities.UserProfiles;
                foreach (var data in UsersData)
                {
                    Models.UserProfile user_profile = new Models.UserProfile();
                    user_profile.Email = data.User.Email;
                    user_profile.UserId = data.UserId;
                    user_profile.ProfilePictureUrl = data.ProfilePictureUrl;
                    user_profile.FirstName = data.FirstName;
                    user_profile.LastName = data.LastName;
                    user_profile.UserName = data.User.Username;
                    user_profile.Mobile = data.Mobile;
                    user_profile.Tokens = data.Tokens;
                    userProfiileList.Add(user_profile);
                }
                return userProfiileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [System.Web.Http.HttpPost]
        [Route("UpdateUserProfile")]
        public async Task<HttpResponseMessage> UpdateUserProfile(UpdateUserProfile updateprofile)
        {
            using (var context = new DiscoveryHunt_DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try          
                    {  
                            var chkUser = (from s in context.Users where s.UserId == updateprofile.UserId select s).FirstOrDefault();
                            if (chkUser == null)
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id = " + updateprofile.UserId.ToString() + "not found");
                            }
                            else
                            {
                                DAL.UserProfile userprof = context.UserProfiles.Where(a=>a.UserId==updateprofile.UserId).FirstOrDefault();
                                userprof.FirstName = updateprofile.FirstName;
                                userprof.LastName = updateprofile.LastName;
                                userprof.Mobile = updateprofile.Mobile;
                                userprof.ProfilePictureUrl= "ProfileImages/dummy-profile-pic.png";
                                userprof.UpdatedDate = DateTime.UtcNow;
                                context.Entry(userprof).State = EntityState.Modified;
                                context.SaveChanges();

                            var query =
                               (from user in context.Users
                               join user_pro in context.UserProfiles on user.UserId equals user_pro.UserId
                               where user.UserId == updateprofile.UserId
                               select new RegisterUserResponse
                               {
                                 Email = user.Email,
                                 Username = user.Username,
                                 FirstName = user_pro.FirstName,
                                 LastName= user_pro.LastName,
                                 ProfilePictureUrl = user_pro.ProfilePictureUrl
                               }).FirstOrDefault();
                                transaction.Commit();
                                return Request.CreateResponse(HttpStatusCode.OK,query);
                            }
                    }
                    catch(Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong");
                    }
                }
            }
        }
    }
}