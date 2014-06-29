using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace Repository.Controllers
{
    [DataContract]
    public class Author
    {
        [DataMember]
        public int Id { get; set ; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }
}