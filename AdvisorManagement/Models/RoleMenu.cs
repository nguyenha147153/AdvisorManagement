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
    
    public partial class RoleMenu
    {
        public int id { get; set; }
        public Nullable<int> id_role { get; set; }
        public Nullable<int> id_menu { get; set; }
    
        public virtual Menu Menu { get; set; }
        public virtual Role Role { get; set; }
    }
}
