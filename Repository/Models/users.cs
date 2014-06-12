//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class users
    {
        public users()
        {
            this.books = new HashSet<books>();
            this.books1 = new HashSet<books>();
            this.comments = new HashSet<comments>();
        }
    
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public byte[] salt { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public int type { get; set; }
        public Nullable<bool> active { get; set; }
        public byte[] image { get; set; }
        public Nullable<bool> isConfirmed { get; set; }
        public string confirmationToken { get; set; }
        public string year { get; set; }
        public string department { get; set; }
        public string aboutme { get; set; }
    
        public virtual ICollection<books> books { get; set; }
        public virtual user_types user_types { get; set; }
        public virtual ICollection<books> books1 { get; set; }
        public virtual ICollection<comments> comments { get; set; }
    }
}
