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
    
    public partial class Produ
    {
        public Produ()
        {
            this.DetaliiFacturas = new HashSet<DetaliiFactura>();
        }
    
        public int IdProdus { get; set; }
        public string NumeProdus { get; set; }
        public string Imagine { get; set; }
        public Nullable<float> Pret { get; set; }
        public string Descriere { get; set; }
        public Nullable<int> Cantitate { get; set; }
        public Nullable<System.DateTime> DataCreare { get; set; }
        public Nullable<bool> Status { get; set; }
        public int IdCategorie { get; set; }
    
        public virtual Categorie Categorie { get; set; }
        public virtual ICollection<DetaliiFactura> DetaliiFacturas { get; set; }
    }
}
