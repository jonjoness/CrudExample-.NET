using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace Entities
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");
            string s1=System.IO.File.ReadAllText("countries.json");
            List<Country> l1=System.Text.Json.JsonSerializer.Deserialize<List<Country>>(s1);
            foreach (Country c in l1)
            {
                modelBuilder.Entity<Country>().HasData(c);
            }
            string s2 = System.IO.File.ReadAllText("persons.json");
            List<Person> l2 = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(s2);

            foreach(Person p in l2)
            {
                modelBuilder.Entity<Person>().HasData(p);
            }
        }
        public List<Person> sp_GetAllPersons()
        { 
        
         return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
    }
}
