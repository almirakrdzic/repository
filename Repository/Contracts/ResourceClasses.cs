using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Repository.Controllers
{
    public class Translation
    {
        public string code { get; set; }
        public string lang { get; set; }
        public List<string> text { get; set; }

    };
    public class Details
    {
        public string download { get; set; }
        public string imgurl { get; set; }
        public string detailurl { get; set; }
        public string isbn { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string elasticid { get; set; }
        public string numofpages { get; set; }
        public string pubdate { get; set; }
        public string publisher { get; set; }
        public string institution { get; set; }
    };
    public class EResource
    {
        public string id { get; set; }
        public string title { get; set; }
        public string download { get; set; }
        public string imgurl { get; set; }
        public string detailurl { get; set; }
        public string score { get; set; }
        public string isbn { get; set; }
        public string highlight { get; set; }
    };

    public class EResult
    {
        public string took { get; set; }
        public string timed_out { get; set; }
        public string num_results { get; set; }
        public List<EResource> results { get; set; }
    };

    public class Resource
    {
        public int id { get; set; }
        public Details details { get; set; }
        public string addedby { get; set; }
        public Double rating { get; set; }
        public int people { get; set; }
    };

    public class Ratings
    {
        public Double rating { get; set; }
        public int people { get; set; }
    };



}