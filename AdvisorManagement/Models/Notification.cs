//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdvisorManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notification
    {
        public int id { get; set; }
        public Nullable<int> id_notification { get; set; }
        public string send_to { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<bool> is_read { get; set; }
        public Nullable<int> id_user { get; set; }
    }
}
