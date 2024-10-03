using AutoMapper;
using Contracts.Dtos.AdministratorDtos;
using Contracts.Dtos.TaskDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile() 
        {
            CreateMap<Domain.Entities.Task, TaskDto>().ReverseMap();
            CreateMap<Domain.Entities.Task, TaskDtoForCreate>().ReverseMap();
            CreateMap<Domain.Entities.Task, TaskDtoForUpdate>();
        }
    }
}
