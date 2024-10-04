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
        }
    }
}
