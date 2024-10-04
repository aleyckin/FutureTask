using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance
{
    public sealed class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProjectUsers> ProjectUsers { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<Column> Columns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryDbContext).Assembly);
        }
    }
}
