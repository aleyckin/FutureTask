using AutoMapper;
using Contracts.Dtos.AdministratorDtos;
using Contracts.Dtos.SpecializationDtos;
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
        }
    }
}
