using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Web;

namespace Repository.Controllers
{
     [DataContract]
    public class Comment
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public User IdUser { get; set; }
        [DataMember]
        public Book IdBook { get; set; }
    }
}