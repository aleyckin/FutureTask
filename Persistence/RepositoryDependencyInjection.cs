using Domain.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositoryDependencies(this IServiceCollection services)
        {
            services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectUsersRepository, ProjectUsersRepository>();
            services.AddScoped<IColumnRepository, ColumnRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMetricsRepository, MetricsRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
