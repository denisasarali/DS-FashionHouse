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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBcasademodaEntities : DbContext
    {
        public DBcasademodaEntities()
            : base("name=DBcasademodaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Articol> Articols { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Culoare> Culoares { get; set; }
        public DbSet<DetaliiFactura> DetaliiFacturas { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Marime> Marimes { get; set; }
        public DbSet<Membru> Membrus { get; set; }
        public DbSet<Produ> Produs { get; set; }
        public DbSet<Programari> Programaris { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
