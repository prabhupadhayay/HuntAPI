using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DiscoveryHuntApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }

    }

    public class LoginUser
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserProfile
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int? Mobile { get; set; }
        public string Tokens { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }

    }

    public class Register
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Enter password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password does not match.")]
        [NotMapped] // Does not effect with your database
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
    public class RegisterUserResponse 
    { 
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        
    }
    public class LoginUserResponse
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }

    }


    public class UpdateUserProfile
    {
        
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public int? Mobile { get; set; }
        public string Tokens { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}