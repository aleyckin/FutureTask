using AutoMapper;
using Contracts.Dtos.AdministratorDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class AdministratorProfile : Profile
    {
        public AdministratorProfile() 
        {
            CreateMap<Administrator, AdministratorDto>();
            CreateMap<Administrator, AdministratorDtoForCreate>().ReverseMap();
            CreateMap<Administrator, AdministratorDtoForUpdate>();
        }
    }
}
