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
    
    public partial class activeSession
    {
        public int sessionId { get; set; }
        public string sessionToken { get; set; }
        public System.DateTime expireTime { get; set; }
        public int userId { get; set; }
    
        public virtual user user { get; set; }
    }
}
