using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Repository.Controllers
{
    [DataContract]
    [Serializable]
    public class User
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = (typeof(Localization.Error)), ErrorMessageResourceName ="UsernameRequired")]
        public string Username { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = (typeof(Localization.Error)), ErrorMessageResourceName = "PasswordRequired")] 
        public string Password { get; set; }
        
        [DataMember]
        public byte[] Salt { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = (typeof(Localization.Error)), ErrorMessageResourceName = "FirstNameRequired")]
        public string FirstName { get; set; }
        
        [DataMember]
        [Required(ErrorMessageResourceType = (typeof(Localization.Error)), ErrorMessageResourceName = "LastNameRequired")]
        public string LastName { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = (typeof(Localization.Error)), ErrorMessageResourceName = "EmailRequired")]
        public string Email { get; set;  }

        [DataMember]
        public UserType Type { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public String Image { get; set; }

        [DataMember]
        public bool IsConfirmed { get; set; }

        [DataMember]
        public string ConfirmationToken { get; set; }

        [DataMember]
        public string Year { get; set; }

        [DataMember]
        public string Department { get; set; }
    }
}