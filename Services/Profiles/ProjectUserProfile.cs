using AutoMapper;
using Contracts.Dtos.ProjectUsersDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class ProjectUserProfile : Profile
    {
        public ProjectUserProfile() 
        {
            CreateMap<ProjectUsers, ProjectUsersDto>().ReverseMap();
            CreateMap<ProjectUsers, ProjectUsersDtoForListProjects>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.RoleOnProject, opt => opt.MapFrom(src => src.RoleOnProject)).ReverseMap();
            CreateMap<ProjectUsers, ProjectUsersDtoForListUsers>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.RoleOnProject, opt => opt.MapFrom(src => src.RoleOnProject)).ReverseMap();
        }
    }
}
