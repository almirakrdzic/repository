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
    
    public partial class authors
    {
        public authors()
        {
            this.books = new HashSet<books>();
        }
    
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string biography { get; set; }
        public Nullable<System.DateTime> birth_date { get; set; }
        public Nullable<bool> active { get; set; }
    
        public virtual ICollection<books> books { get; set; }
    }
}