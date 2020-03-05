using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiscoveryHuntApi.Models
{
    public class Questions
    {
        [Required(ErrorMessage = "Enter Trail ID")]
        public int TrailId { get; set; }
      
        public int? QuestionId { get; set; }
        [Required(ErrorMessage = "Enter Question Name")]
        public string QuestionName { get; set; }
        [Required(ErrorMessage = "Enter Option A")]
        public string OptionA { get; set; }
        [Required(ErrorMessage = "Enter Option B")]
        public string OptionB { get; set; }
        [Required(ErrorMessage = "Enter Option C")]
        public string OptionC { get; set; }
        [Required(ErrorMessage = "Enter Option D")]
        public string OptionD { get; set; }
        [Required(ErrorMessage = "Enter Correct Answer")]
        public string CorrectAnswer { get; set; }
        public int? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class Trail
    {
      
        public int? TrailId { get; set; }
        [Required(ErrorMessage = "Enter Trail Name")]
        public string TrailName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class TrailQuestions
    {
        public int TrailId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }
        public int Status { get; set; }
    }

    public class GetTrailByIdModel
    {
        public int? trailId { get; set; }
    }
}