//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PamSolution
{
    using System;
    using System.Collections.Generic;
    
    public partial class serverO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public serverO()
        {
            this.automationScripts = new HashSet<automationScript>();
            this.servers = new HashSet<server>();
            this.servers1 = new HashSet<server>();
        }
    
        public int serverOsId { get; set; }
        public string osName { get; set; }
        public int osFamilyId { get; set; }
        public string osDescription { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<automationScript> automationScripts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<server> servers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<server> servers1 { get; set; }
        public virtual serverOsFamily serverOsFamily { get; set; }
    }
}
