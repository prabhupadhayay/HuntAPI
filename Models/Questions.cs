using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiscoveryHuntApi.Models
{
    public class Questions
    {
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }
        public int Status { get; set; }

    }

    public class Trail
    {
        public int TrailId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}