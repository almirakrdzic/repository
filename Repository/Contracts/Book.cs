using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Repository.Controllers
{
    [DataContract]
    public class Book
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Title { get;  set; }
        [DataMember]
        public string ISBN { get; set;  }
        [DataMember]
        public string Edition { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public User AddedBy { get; set; }
        [DataMember]
        public Double Rating { get; set; }
        [DataMember]
        public int People { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }

        [DataMember]
        public byte[] Content { get; set; }

    }
}