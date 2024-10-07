using AutoMapper;
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
            CreateMap<Domain.Entities.Task, TaskDtoForCreate>();
            CreateMap<TaskDtoForCreate, Domain.Entities.Task>()
                .ForMember(dest => dest.DateEnd, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.DateEnd, DateTimeKind.Utc)));
            CreateMap<Domain.Entities.Task, TaskDtoForUpdate>();
            CreateMap<TaskDtoForUpdate, Domain.Entities.Task>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
