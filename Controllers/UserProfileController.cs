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
using System.Web;

namespace DiscoveryHuntApi.Controllers
{
    public class UserProfileController : ApiController  
    {
        string url = System.Configuration.ConfigurationManager.AppSettings["localUrl"];
        DiscoveryHunt_DBEntities discoveryHunt_DBEntities = new DiscoveryHunt_DBEntities();
        [System.Web.Http.HttpGet]
        [Route("GetUsers")]
        public async Task<HttpResponseMessage> Get([FromUri]UserProfileRequestParam param)
        {
            try
            {
                if(param != null)
                {
                    if (param.userid.HasValue)
                    {
                        Models.UserProfile userpro = new Models.UserProfile();
                        var singleUserdata = discoveryHunt_DBEntities.UserProfiles.Where(a => a.UserId == param.userid).SingleOrDefault();
                        userpro.FirstName = singleUserdata.FirstName;
                        userpro.LastName = singleUserdata.LastName;
                        userpro.UserName = singleUserdata.User.Username;
                        userpro.Mobile = singleUserdata.Mobile;
                        userpro.Tokens = singleUserdata.Tokens;
                        return Request.CreateResponse(HttpStatusCode.OK, userpro);
                    }

                }
                List<Models.UserProfile> userProfiileList = new List<Models.UserProfile>();
                var UsersData = discoveryHunt_DBEntities.UserProfiles;

                foreach (var data in UsersData)
                {
                    Models.UserProfile user_profile = new Models.UserProfile();
                    user_profile.Email = data.User.Email;
                    user_profile.UserId = data.UserId;

                    user_profile.ProfilePictureUrl = url + data.ProfilePictureUrl;
                    user_profile.FirstName = data.FirstName;
                    user_profile.LastName = data.LastName;
                    user_profile.UserName = data.User.Username;
                    user_profile.Mobile = data.Mobile;
                    user_profile.Tokens = data.Tokens;
                    userProfiileList.Add(user_profile);
                }
                return Request.CreateResponse(HttpStatusCode.OK, userProfiileList);
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
                            DAL.UserProfile userprof = context.UserProfiles.Where(a => a.UserId == updateprofile.UserId).FirstOrDefault();
                            userprof.FirstName = updateprofile.FirstName;
                            userprof.LastName = updateprofile.LastName;
                            userprof.Mobile = updateprofile.Mobile;
                            userprof.ProfilePictureUrl = "ProfileImages/dummy-profile-pic.png";
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
                                    LastName = user_pro.LastName,
                                    ProfilePictureUrl = user_pro.ProfilePictureUrl
                                }).FirstOrDefault();
                            transaction.Commit();
                            return Request.CreateResponse(HttpStatusCode.OK, query);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong");
                    }
                }
            }
        }

        [System.Web.Http.HttpPost]
        [Route("UpdateUserProfileImage")]
        public async Task<HttpResponseMessage> PostUserImage([FromUri]ProfileImageModel profimg)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            using (var context = new DiscoveryHunt_DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    { 
                var chkUser = (from s in context.Users where s.UserId == profimg.UserId select s).FirstOrDefault();
                if (chkUser == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id = " + profimg.UserId.ToString() + "not found");
                }

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                                 string imageName = Guid.NewGuid().ToString() + extension;
                                 var filePath = HttpContext.Current.Server.MapPath("~/ProfileImages/" + imageName);
                                 postedFile.SaveAs(filePath);
                                 DAL.UserProfile userprof = context.UserProfiles.Where(a => a.UserId == profimg.UserId).FirstOrDefault();
                                 userprof.ProfilePictureUrl = "ProfileImages/" + imageName;
                                 userprof.UpdatedDate = DateTime.UtcNow;
                                 context.Entry(userprof).State = EntityState.Modified;
                                 context.SaveChanges();
                                 transaction.Commit();
                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("some Message");
                dict.Add("error", res);
                transaction.Rollback();
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }

                }
            }

        }



    }
}