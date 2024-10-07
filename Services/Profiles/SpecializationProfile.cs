using AutoMapper;
using Contracts.Dtos.SpecializationDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class SpecializationProfile : Profile
    {
        public SpecializationProfile() 
        {
            CreateMap<Specialization, SpecializationDto>().ReverseMap();
            CreateMap<Specialization, SpecializationDtoForCreate>().ReverseMap();
            CreateMap<Specialization, SpecializationDtoForUpdate>();
            CreateMap<SpecializationDtoForUpdate, Specialization>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
