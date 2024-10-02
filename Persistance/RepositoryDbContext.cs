using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance
{
    public sealed class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryDbContext).Assembly);
        }
    }
}
