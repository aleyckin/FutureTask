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
    public class CheckAccessRights : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly RoleOnProject _roleOnProject;

        public CheckAccessRights(RoleOnProject roleOnProject)
        {
            _roleOnProject = roleOnProject;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.User.FindFirst("userId")?.Value;
            var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (userRole == UserRole.Administrator.ToString())
            {
                return;
            }

            var projectId = context.HttpContext.Request.RouteValues["projectId"]?.ToString();
            if (string.IsNullOrEmpty(projectId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var projectUserService = context.HttpContext.RequestServices.GetRequiredService<IProjectUsersService>();
            var roleOnProject = projectUserService.GetUserRoleOnProject(new Guid(userId), new Guid(projectId)).Result;
            if (roleOnProject != _roleOnProject)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
