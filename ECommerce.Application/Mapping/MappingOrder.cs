using AutoMapper;
using ECommerce.Application.DTOS.OrderDTO;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Mapping
{
    public class MappingOrder : Profile
    {
        public MappingOrder()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName,opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName,opt => opt.MapFrom(src => src.Product.Name));
        }
    }
}
