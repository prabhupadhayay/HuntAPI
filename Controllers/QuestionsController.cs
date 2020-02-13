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
        [Route("AddQuestions")]
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
                            //DAL.Trial trial = context.Trials.Where(a => a.TrialId == questions.TrailId).FirstOrDefault();
                            DAL.Question question = new DAL.Question();
                            question.TrailId = questions.TrailId;
                            question.QuestionName = questions.QuestionName;
                            question.OptionA = questions.OptionA;
                            question.OptionB = questions.OptionB;
                            question.OptionC = questions.OptionC;
                            question.OptionD = questions.OptionD;
                            question.CorrectAnswer = questions.CorrectAnswer;
                            question.CreatedAt = DateTime.UtcNow;
                            context.Questions.Add(question);
                            context.SaveChanges();
                            transaction.Commit();

                            return Request.CreateResponse(HttpStatusCode.OK, "Questions Added Successfully");
                        }
                        var errors = new List<string>();
                        foreach (var state in ModelState)
                        {
                            foreach (var error in state.Value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        var errorList = ModelState.ToDictionary(kvp => kvp.Key.Replace("questions.", ""), kvp => kvp.Value.Errors[0].ErrorMessage);

                        return Request.CreateResponse(HttpStatusCode.OK, errorList);
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        return Request.CreateErrorResponse(HttpStatusCode.NoContent, "Something Went Wrong");
                    }
                }
            }
        }

        [HttpPut]
        [Route("UpdateQuestions")]
        public async Task<HttpResponseMessage> UpdateQuestions(Questions questions)
        {
            using (var context = new DiscoveryHunt_DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var chkQuestion = (from s in context.Questions where s.QuestionId == questions.QuestionId select s).FirstOrDefault();
                            if (chkQuestion == null)
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Question with id = " + questions.QuestionId.ToString() + "not found");
                            }

                            // DAL.Trial trial_data = context.Trials.Where(a => a.TrialId == questions.TrailId).FirstOrDefault();

                            DAL.Question question = context.Questions.Where(a => a.QuestionId == questions.QuestionId).FirstOrDefault();
                            question.TrailId = questions.TrailId;
                            question.QuestionName = questions.QuestionName;
                            question.OptionA = questions.OptionA;
                            question.OptionB = questions.OptionB;
                            question.OptionC = questions.OptionC;
                            question.OptionD = questions.OptionD;
                            question.CorrectAnswer = questions.CorrectAnswer;
                            question.UpdatedAt = DateTime.UtcNow;
                            context.Entry(question).State = EntityState.Modified;
                            context.SaveChanges();
                            transaction.Commit();

                            return Request.CreateResponse(HttpStatusCode.OK, "Questions Updated Successfully");
                        }
                        var errors = new List<string>();
                        foreach (var state in ModelState)
                        {
                            foreach (var error in state.Value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        var errorList = ModelState.ToDictionary(kvp => kvp.Key.Replace("questions.", ""), kvp => kvp.Value.Errors[0].ErrorMessage);

                        return Request.CreateResponse(HttpStatusCode.OK, errorList);
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
