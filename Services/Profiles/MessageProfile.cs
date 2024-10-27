using AutoMapper;
using Contracts.Dtos.MessageDtos;
using Domain.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile() 
        {
            CreateMap<Message, MessageDto>().ReverseMap();
        }
    }
}
