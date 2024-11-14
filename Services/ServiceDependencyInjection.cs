using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using Services.Services;
using Services.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<ISpecializationService, SpecializationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectUsersService, ProjectUsersService>();
            services.AddScoped<IColumnService, ColumnService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IValidatorManager, ValidatorManager>();

            services.AddAutoMapper(typeof(AssemblyReference));

            return services;
        }
    }
}
