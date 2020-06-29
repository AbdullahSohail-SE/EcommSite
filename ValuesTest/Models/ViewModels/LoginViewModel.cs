using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Web.Models;

namespace Web.Models.ViewModels
{
    public class LoginViewModel :BaseModel
    {
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Display(Name ="Remember me")]
        public bool RememberMe { get; set; }

    }
}