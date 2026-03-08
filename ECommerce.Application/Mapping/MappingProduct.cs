using AutoMapper;
using ECommerce.Application.DTOS.ProductDTO;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Mapping
{
    public class MappingProduct :Profile
    {
        public MappingProduct()
        {

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<ProductInput, Product>();
            CreateMap<ProductUpdateInput, Product>();

        }
    }
}
