   using Microsoft.EntityFrameworkCore;
   using FamilyTreeApp.Server.Infrastructure.Models;

namespace FamilyTreeApp.Server.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PersonRelationship> PersonRelationships { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Relationship> Relationship { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PersonRelationship>()
                .HasKey(pr => pr.PersonId); // Ensure the primary key is configured
            modelBuilder.Entity<PersonRelationship>().ToView("v_PersonRelationhip");
        }
    }
}
   