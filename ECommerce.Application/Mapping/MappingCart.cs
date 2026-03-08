using AutoMapper;
using ECommerce.Application.DTOS.CartDTO;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Mapping
{
    public class MappingCart : Profile
    {
        public MappingCart() 
        {
            CreateMap<CartItem,CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price,opt => opt.MapFrom(src => src.Product.Price));

            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Id,opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Items,opt => opt.MapFrom(src => src.Items))
                .ForMember(dest=>dest.UserName,opt=>opt.MapFrom(src=>src.User.UserName));

            CreateMap<CartInput, Cart>();

            CreateMap<CartUpdateInput, Cart>();

            CreateMap<CartItemInput, CartItem>();



        }
    }
}
