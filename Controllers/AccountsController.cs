using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscoveryHuntApi.DAL;
using DiscoveryHuntApi.Models;
using DiscoveryHuntApi.Helper;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Data.Entity;
using DiscoveryHuntApi.Helpers;


namespace DiscoveryHuntApi.Controllers
{
    public class AccountsController : ApiController
    {
        

        [HttpPost]
        [Route("Registration")]
        public async Task<HttpResponseMessage> Registration(Register regUser)
        {
            using (var context = new DiscoveryHunt_DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var chkUser = (from s in context.Users where s.Username == regUser.UserName || s.Email == regUser.Email select s).FirstOrDefault();
                            if (chkUser == null)
                            {
                                DAL.User user = new DAL.User();
                                var keyNew = Helper.Helper.GeneratePassword(50);
                                var password = Helper.Helper.EncodePassword(regUser.Password, keyNew);
                                user.Password = password;
                                user.CreatedDate = DateTime.Now;
                                user.UpdatedDate = DateTime.Now;
                                user.PasswordSalt = keyNew;
                                user.Email = regUser.Email;
                                user.Username = regUser.UserName;
                                context.Users.Add(user);
                                context.SaveChanges();
                               
                                DAL.UserProfile userprof = new DAL.UserProfile();
                                userprof.UserId = user.UserId;
                                userprof.FirstName = regUser.FirstName;
                                userprof.LastName = regUser.LastName;
                                userprof.ProfilePictureUrl = "ProfileImages/dummy-profile-pic.png";
                                context.UserProfiles.Add(userprof);
                                context.SaveChanges();
                                transaction.Commit();

                                RegisterUserResponse reguserResponse = new RegisterUserResponse();
                                reguserResponse.Email = regUser.Email;
                                reguserResponse.FirstName = regUser.FirstName;
                                reguserResponse.LastName = regUser.LastName;
                                reguserResponse.ProfilePictureUrl = "ProfileImages/dummy-profile-pic.png";
                                reguserResponse.Username = regUser.UserName;
                                return Request.CreateResponse(HttpStatusCode.OK, reguserResponse);
                            }
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User already Exist");
                        }
                        var errors = new List<string>();
                        foreach (var state in ModelState)
                        {
                            foreach (var error in state.Value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        var errorList = ModelState.ToDictionary(kvp => kvp.Key.Replace("regUser.", ""), kvp => kvp.Value.Errors[0].ErrorMessage);

                        return Request.CreateResponse(HttpStatusCode.OK, errorList);
                    }
                    catch
                    {
                        transaction.Rollback();
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong");
                    }

                    //return Request.CreateResponse(HttpStatusCode.Forbidden, errors);
                }
            }

        }
        [HttpPost]
        [Route("Login")]
        public async Task<HttpResponseMessage> Login(string userName, string password)
        {
            try
            {
                using (var context = new DiscoveryHunt_DBEntities())
                {
                    if (ModelState.IsValid)
                    {
                        var loggedinUser = (from s in context.Users where s.Username == userName || s.Email == userName select s).FirstOrDefault();
                    if (loggedinUser != null)
                    {
                        var hashCode = loggedinUser.PasswordSalt;
                        //Password Hasing Process Call Helper Class Method    
                        var encodingPasswordString = Helper.Helper.EncodePassword(password, hashCode);
                        //Check Login Detail User Name Or Password    
                        var user_info = (from s in context.Users where (s.Username == userName || s.Email == userName) && s.Password.Equals(encodingPasswordString) select s).FirstOrDefault();
                        if (user_info != null)
                        {
                            //RedirectToAction("Details/" + id.ToString(), "FullTimeEmployees");    
                            //return View("../Admin/Registration"); url not change in browser

                            LoginUserResponse logginUser = new LoginUserResponse();
                            logginUser.Email = user_info.Email;
                            logginUser.Username = user_info.Username;
                            logginUser.ProfilePictureUrl = "ProfileImages/dummy-profile-pic.png";
                            logginUser.FirstName = user_info.UserProfile.FirstName;
                            logginUser.LastName = user_info.UserProfile.LastName;

                            return Request.CreateResponse(HttpStatusCode.OK, logginUser);
                        }
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "UserName or Password is Incorrect");
                        }
                }
                    var errors = new List<string>();
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    var errorList = ModelState.ToDictionary(kvp => kvp.Key.Replace("userName.", ""), kvp => kvp.Value.Errors[0].ErrorMessage);

                    return Request.CreateResponse(HttpStatusCode.OK, errorList);
                    //return Request.CreateResponse(HttpStatusCode.Forbidden, errors);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<HttpResponseMessage> ForgotPassword(string UserName)
        {

            using (var context = new DiscoveryHunt_DBEntities())
            {
                //check user existance
                var user = (from s in context.Users where s.Username == UserName select s).FirstOrDefault();
            if (user == null)
            {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User Does not Exsist");
                }
            else
            {
                    

                    //generate password token
                    var user_id = user.UserId;
                    DAL.User userd = context.Users.Where(a => a.UserId == user.UserId).FirstOrDefault();
                    userd.PasswordResetToken = Guid.NewGuid().ToString();
                    userd.TimeStamp = DateTime.UtcNow;
                    context.Entry(userd).State = EntityState.Modified;
                    context.SaveChanges();
                    //create url with above token
                    // var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { un = UserName, rt = token }, "http") + "'>Reset Password</a>";
                    var resetLink = this.Url.Link("Default", new { Controller = "Account", Action = "ResetPassword", un = user_id, rt = userd.PasswordResetToken });

                    //get user emailid

                    var emailid = (from i in context.Users
                               where i.Username == UserName
                               select i.Email).FirstOrDefault();
                //send mail
                string subject = "Password Reset Token";
                string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; //edit it
                try
                {
                        Mail.SendEMail(emailid, subject, body);
                           
                }
                catch (Exception ex)
                {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong");
                    }
                //only for testing
                
            }
        }
            return Request.CreateResponse(HttpStatusCode.OK, "query");

        }


    }
}