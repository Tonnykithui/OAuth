
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Models
{
    public class RegisterUser
    {
        public int UserId { get; set; }

        public string Fname { get; set; }
        
        public string Lname { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password should not be less than 5 characters")]
        [MaxLength(15, ErrorMessage = "Password should not be greater than 15 characters")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
