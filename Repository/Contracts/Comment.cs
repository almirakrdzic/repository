using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Repository.Controllers
{
     [Serializable]
    [DataContract]
    public class Comment
    {        
        public int Id { get; set; }

        [DataMember(IsRequired = true)]  
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Text { get; set; }       
         
        [DataMember]
        public User IdUser { get; set; }        
       
         [DataMember(IsRequired=true)]       
        public int IdBook { get; set; }
    }
}