﻿using AutoMapper;
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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserDtoForCreate>().ReverseMap();
            CreateMap<User, UserDtoForUpdate>();
        }
    }
}
