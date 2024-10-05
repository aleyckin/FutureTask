using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Web
{
    public static class MigrationsExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder builder)
        {
            using IServiceScope scope = builder.ApplicationServices.CreateScope();
            using RepositoryDbContext dbContext = 
                scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
