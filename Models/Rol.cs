//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CasadeModa.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rol
    {
        public Rol()
        {
            this.Membrus = new HashSet<Membru>();
        }
    
        public int IdRol { get; set; }
        public string NumeRol { get; set; }
    
        public virtual ICollection<Membru> Membrus { get; set; }
    }
}
