using DiscoveryHuntApi.DAL;
using DiscoveryHuntApi.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiscoveryHuntApi.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult ResetPassword(int un, string rt)
        {
            using (var context = new DiscoveryHunt_DBEntities())
            {
                // UsersContext db = new UsersContext();

                //TODO: Check the un and rt matching and then perform following
                //get userid of received username
                var userid = (from i in context.Users
                              where i.UserId == un
                              select i.UserId).FirstOrDefault();
                //check userid and token matches
                var users_detail = context.Users.Where(a => a.UserId == userid).FirstOrDefault();
                DateTime expiration = (DateTime)users_detail.TimeStamp;
                DateTime expiration1 = expiration.AddMinutes(20);
                DateTime currentTime = DateTime.UtcNow;
                if(expiration1 < currentTime)
                {
                    ViewBag.info = "Your Link has been Expired";
                    return View();
                }

                bool any = (from j in context.Users
                            where (j.UserId == userid)
                            && (j.PasswordResetToken == rt)
                            &&(j.TimeStamp < expiration)
                            //&& (j.PasswordVerificationTokenExpirationDate < DateTime.Now)
                            select j).Any();

                if (any == true)
                {
                    //generate random password
                    string key = Helper.Helper.GeneratePassword(50);
                    string randomPassword= Helper.Helper.GeneratePassword(8);
                    var password = Helper.Helper.EncodePassword(randomPassword, key);
                    DAL.User userd = context.Users.Where(a => a.UserId == userid).FirstOrDefault();
                    userd.Password = password;
                    userd.PasswordSalt = key;
                    userd.UpdatedDate = DateTime.UtcNow;
                    context.Entry(userd).State = EntityState.Modified;
                    context.SaveChanges();
                    //reset password
                    var response = randomPassword;
                    if (response != null)
                    {
                        //get user emailid to send password
                        var emailid = (from i in context.Users
                                       where i.UserId == un
                                       select i.Email).FirstOrDefault();
                        //send email
                        string subject = "New Password";
                        string body = "<b>Please find the New Password</b><br/>" + randomPassword; //edit it
                        try
                        {
                            Mail.SendEMail(emailid, subject, body);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        //display message

                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            ViewBag.info = "Link has been Sent to your registered Email";
            return View();
        }
    }
}