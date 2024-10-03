using AutoMapper;
using Contracts.Dtos.AdministratorDtos;
using Contracts.Dtos.ColumnDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class ColumnProfile : Profile
    {
        public ColumnProfile() 
        {
            CreateMap<Column, ColumnDto>().ReverseMap();
            CreateMap<Column, ColumnDtoForCreate>().ReverseMap();
            CreateMap<Column, ColumnDtoForUpdate>();
        }
    }
}
