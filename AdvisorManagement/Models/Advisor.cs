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
    
    public partial class Advisor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Advisor()
        {
            this.VLClass = new HashSet<VLClass>();
        }
    
        public string advisor_code { get; set; }
        public Nullable<int> status_id { get; set; }
        public Nullable<int> account_id { get; set; }
    
        public virtual AccountUser AccountUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VLClass> VLClass { get; set; }
    }
}
