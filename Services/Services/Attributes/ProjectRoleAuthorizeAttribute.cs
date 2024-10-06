using Domain.Entities.Enums;
using Domain.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Attributes
{
    public class ProjectRoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly RoleOnProject _roleOnProject;

        public ProjectRoleAuthorizeAttribute(RoleOnProject roleOnProject)
        {
            _roleOnProject = roleOnProject;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.User.FindFirst("userId")?.Value;

            if (userId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var projectId = context.HttpContext.Request.RouteValues["projectId"]?.ToString();

            if (string.IsNullOrEmpty(projectId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var serviceManager = context.HttpContext.RequestServices.GetRequiredService<IServiceManager>();
            var roleOnProject = serviceManager.ProjectUsersService.GetUserRoleOnProject(new Guid(userId), new Guid(projectId)).Result;

            if (roleOnProject != _roleOnProject)
            {
                throw new UserPermitionException();
            }
        }
    }
}
