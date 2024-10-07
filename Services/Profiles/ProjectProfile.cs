using AutoMapper;
using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile() 
        {
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<Project, ProjectDtoForCreate>().ReverseMap();
            CreateMap<Project, ProjectDtoForUpdate>();
            CreateMap<ProjectDtoForUpdate, Project>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
