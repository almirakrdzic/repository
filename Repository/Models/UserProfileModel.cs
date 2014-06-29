using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Repository.Models
{
    [Serializable]
    public class UserProfileModel
    {
        [Display(Name = "Username")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "First Name")]
        [Required]    
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        [Required]       
        public string LastName { get; set; }


        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required]       
        public string Email { get; set; }

        public String Image { get; set; }

        [Display(Name = "AboutMe")]
        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(255)]
        public string AboutMe { get; set; }
        
        public int Year { get; set; }        
        
        public string Department { get; set; }


    }
}