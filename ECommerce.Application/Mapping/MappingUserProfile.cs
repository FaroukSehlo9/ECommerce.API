using AutoMapper;
using ECommerce.Application.DTOS.AuthDTO;
using ECommerce.Application.DTOS.UserDTO;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Mapping
{
    public class MappingUserProfile : Profile
    {
        public MappingUserProfile()
        {

            CreateMap<RegistrationRequest, User>();
            CreateMap<UserInput, User>();
            CreateMap<UserUpdateInput, User>();

        }
    }
}
