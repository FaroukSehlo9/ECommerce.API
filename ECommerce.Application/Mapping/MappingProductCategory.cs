using AutoMapper;
using ECommerce.Application.DTOS.ProductCategoryDTO;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Mapping
{
    public class MappingProductCategory : Profile
    {
        public MappingProductCategory() 
        {
            CreateMap<ProductCategory, ProductCategoryDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<ProductCategoryInput, ProductCategory>();
            CreateMap<ProductCategoryUpdateInput, ProductCategory>();
        }
    }
}
