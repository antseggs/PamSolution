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
    
    public partial class server
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public server()
        {
            this.protectedAccounts = new HashSet<protectedAccount>();
            this.serverAccessLevels = new HashSet<serverAccessLevel>();
        }
    
        public int serverId { get; set; }
        public string serverName { get; set; }
        public int serverOsId { get; set; }
        public string serverDescription { get; set; }
        public string serverIp { get; set; }
        public Nullable<bool> ipStatic { get; set; }
        public string fqdn { get; set; }
        public string serverNotes { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<protectedAccount> protectedAccounts { get; set; }
        public virtual serverO serverO { get; set; }
        public virtual serverO serverO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<serverAccessLevel> serverAccessLevels { get; set; }
    }
}
