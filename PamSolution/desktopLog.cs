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
    
    public partial class desktopLog
    {
        public int desktopLogId { get; set; }
        public int userId { get; set; }
        public System.DateTime accessTime { get; set; }
        public string logContentLocation { get; set; }
        public int permissionLevelId { get; set; }
        public Nullable<System.DateTime> finishTime { get; set; }
        public string userNote { get; set; }
        public int protectedAccountId { get; set; }
    
        public virtual permissionLevel permissionLevel { get; set; }
        public virtual protectedAccount protectedAccount { get; set; }
        public virtual protectedAccount protectedAccount1 { get; set; }
        public virtual user user { get; set; }
    }
}
