using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DiscoveryHuntApi.Models;
using DiscoveryHuntApi.DAL;
using System.Data.Entity;

namespace DiscoveryHuntApi.Controllers
{
    public class QuestionsController : ApiController
    {

        [HttpPost]
        [Route("Questions")]
        public async Task<HttpResponseMessage> Questions(Questions questions)
        {
            using (var context = new DiscoveryHunt_DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            DAL.Question question = new DAL.Question();
                            question.QuestionName = questions.QuestionName;
                            question.OptionA = questions.OptionA;
                            question.OptionB = questions.OptionB;
                            question.OptionC = questions.OptionC;
                            question.OptionD = questions.OptionD;
                            question.CorrectAnswer = questions.CorrectAnswer; 
                            context.Questions.Add(question);
                            context.SaveChanges();
                            transaction.Commit();

                            return Request.CreateResponse(HttpStatusCode.OK, "Questions Added Successfully");
                        }
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "");
                    }
                    catch 
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong");
                    }
                }
            }
        }
    }
}
