using AutoMapper;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.ProductCategoryDTO;
using ECommerce.Application.IService;
using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IMapper _Imapper;
        private readonly IStringLocalizer<GeneralMessages> _localization;
        private readonly IUnitOfWork _unit;
        public ProductCategoryService(IMapper imapper, IStringLocalizer<GeneralMessages> localization, IUnitOfWork unitOfWork)
        {
            _Imapper = imapper;
            _localization = localization;
            _unit = unitOfWork;
        }

        public async Task<GeneralResponse<List<ProductCategoryDto>>> GetAll()
        {

            var result = _unit.ProductCategory.All().Include(x => x.Product).Include(x => x.Category).ToList().Select(x => new ProductCategoryDto
            {
                Id = x.Id,
                ProductId=x.ProductId,
                ProductName=x.Product.Name,
                CategoryId=x.CategoryId,
                CategoryName=x.Category.Name,

            }).ToList();
            return new GeneralResponse<List<ProductCategoryDto>>(result, _localization["Succes"].Value, result.Count());
        }

        public async Task<GeneralResponse<ProductCategoryDto>> GetByIdAsync(Guid Id)
        {
            var result = _unit.ProductCategory.All().Include(x => x.Product).Include(x => x.Category).Where(d => d.Id == Id).ToList().Select(x => new ProductCategoryDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name

            }).FirstOrDefault();
            return new GeneralResponse<ProductCategoryDto>(result, _localization["Succes"].Value);
        }
        public async Task<GeneralResponse<ProductCategoryDto>> GetByProudctIdAsync(Guid ProductId)
        {
            var result = _unit.ProductCategory.All().Include(x => x.Product).Include(x => x.Category).Where(x=>x.ProductId==ProductId).ToList().Select(x => new ProductCategoryDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name

            }).FirstOrDefault();
            return new GeneralResponse<ProductCategoryDto>(result, _localization["Succes"].Value);
        }
        public async Task<GeneralResponse<ProductCategoryDto>> GetByCategoryIdAsync(Guid CategoryId)
        {
            var result = _unit.ProductCategory.All().Include(x => x.Product).Include(x => x.Category).Where(x=>x.CategoryId==CategoryId).ToList().Select(x => new ProductCategoryDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name

            }).FirstOrDefault();
            return new GeneralResponse<ProductCategoryDto>(result, _localization["Succes"].Value);
        }
        public async Task<GeneralResponse<Guid>> Add(ProductCategoryInput Input, Guid UserId)
        {
            try
            {
                var ProductCategory = _Imapper.Map<ProductCategoryInput, ProductCategory>(Input);
                ProductCategory.CreatedBy = UserId;
                ProductCategory.CreationDate = DateTime.Now;
                if (!ValidProductCategory(ProductCategory, out string message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                await _unit.ProductCategory.AddAsync(ProductCategory);
                var result = _unit.Save();
                return result > 0 ? new GeneralResponse<Guid>(ProductCategory.Id, _localization["succesfully"].Value)
                     : new GeneralResponse<Guid>(_localization["errorinsave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }

        public async Task<GeneralResponse<Guid>> Update(ProductCategoryUpdateInput Input, Guid UserId)
        {
            try
            {
                ProductCategory OldProductCategory = await _unit.ProductCategory.GetByIdAsync(Input.Id);
                var ProductCategory = _Imapper.Map<ProductCategoryUpdateInput, ProductCategory>(Input, OldProductCategory);
                ProductCategory.UpdatedBy = UserId;
                ProductCategory.UpdatedDate = DateTime.Now;
                if (!ValidProductCategory(ProductCategory, out string message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                await _unit.ProductCategory.UpdateAsync(ProductCategory);
                var result = _unit.Save();
                return result >= 1 ? new GeneralResponse<Guid>(ProductCategory.Id, _localization["UpdatedSuccesfully"].Value)
                   : new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {
            await _unit.ProductCategory.SoftDelete(Id);
            var results = await _unit.SaveAsync();

            return results >= 1 ? new GeneralResponse<Guid>(Id, _localization["DeletedSuccesfully"].Value) :
                new GeneralResponse<Guid>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);
        }

        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {
            await _unit.ProductCategory.SoftDeleteRangeAsync(Id);

            var results = _unit.Save();

            return results >= 1 ? new GeneralResponse<List<Guid>>(Id, _localization["DeletedSuccesfully"].Value) :
                 new GeneralResponse<List<Guid>>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);

        }

        public bool ValidProductCategory(ProductCategory Input, out string message)
        {
            if (_unit.ProductCategory.All().Where(x => x.ProductId == Input.ProductId&&x.CategoryId==Input.CategoryId && x.Id != Input.Id).Any())
            {
                message = "ProductCategoryFoundBefore";
                return false;
            }
            message = "Done";
            return true;
        }
    }
}
