using AutoMapper;
using ECommerce.Application.DTOS.CategoryDTO;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Mapping
{
    public class MappingCategory : Profile
    {
        public MappingCategory() 
        {
            CreateMap<CategoryDto, Category>();
            CreateMap<CategoryInput, Category>();
            CreateMap<CategoryUpdateInput, Category>();
        }
    }
}
