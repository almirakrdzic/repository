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
        public string Username { get; set; }

        [Display(Name = "First Name")]       
        public string FirstName { get; set; }

       
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Email")]        
        public string Email { get; set; }

        [Display(Name = "AboutMe")]       
        public string AboutMe { get; set; }
       
    }
}