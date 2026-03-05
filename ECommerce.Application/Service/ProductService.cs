using AutoMapper;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.ProductDTO;
using ECommerce.Application.IService;
using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service
{
    public class ProductService : IProductService
    {
        private readonly IMapper _Imapper;
        private readonly IStringLocalizer<GeneralMessages> _localization;
        private readonly IUnitOfWork _unit;
        public ProductService(IMapper imapper, IStringLocalizer<GeneralMessages> localization, IUnitOfWork unitOfWork)
        {
            _Imapper = imapper;
            _localization = localization;
            _unit = unitOfWork;
        }

        public async Task<GeneralResponse<List<ProductDto>>> GetAll()
        {
          
            var result = _unit.Product.All().ToList().Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                StockQuantity = x.StockQuantity,
                UserId = x.UserId,
                UserName=_unit.User.All().Where(u=>u.Id==x.UserId).Select(u=>u.UserName).FirstOrDefault()

            }).ToList();
            return new GeneralResponse<List<ProductDto>>(result, _localization["Succes"].Value, result.Count());
        }

        public async Task<GeneralResponse<ProductDto>> GetByIdAsync(Guid Id)
        {
            var result = _unit.Product.All().Where(d => d.Id == Id).ToList().Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                StockQuantity = x.StockQuantity,
                UserId = x.UserId,
                UserName = _unit.User.All().Where(u => u.Id == x.UserId).Select(u => u.UserName).FirstOrDefault()

            }).FirstOrDefault();
            return new GeneralResponse<ProductDto>(result, _localization["Succes"].Value);
        }

        public async Task<GeneralResponse<Guid>> Add(ProductInput Input, Guid UserId)
        {
            try
            {
                var Product = _Imapper.Map<ProductInput, Product>(Input);
                Product.CreatedBy = UserId;
                Product.CreationDate = DateTime.Now;
                if (!ValidProduct(Product, out string message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                await _unit.Product.AddAsync(Product);
                var result = _unit.Save();
                return result > 0 ? new GeneralResponse<Guid>(Product.Id, _localization["succesfully"].Value)
                     : new GeneralResponse<Guid>(_localization["errorinsave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }

        public async Task<GeneralResponse<Guid>> Update(ProductUpdateInput Input, Guid UserId)
        {
            try
            {
                Product OldProduct = await _unit.Product.GetByIdAsync(Input.Id);
                var Product = _Imapper.Map<ProductUpdateInput, Product>(Input, OldProduct);
                Product.UpdatedBy = UserId;
                Product.UpdatedDate = DateTime.Now;
                if (!ValidProduct(Product, out string message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                await _unit.Product.UpdateAsync(Product);
                var result = _unit.Save();
                return result >= 1 ? new GeneralResponse<Guid>(Product.Id, _localization["UpdatedSuccesfully"].Value)
                   : new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {
            await _unit.Product.SoftDelete(Id);
            var results = await _unit.SaveAsync();

            return results >= 1 ? new GeneralResponse<Guid>(Id, _localization["DeletedSuccesfully"].Value) :
                new GeneralResponse<Guid>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);
        }

        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {
            await _unit.Product.SoftDeleteRangeAsync(Id);

            var results = _unit.Save();

            return results >= 1 ? new GeneralResponse<List<Guid>>(Id, _localization["DeletedSuccesfully"].Value) :
                 new GeneralResponse<List<Guid>>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);

        }

        public bool ValidProduct(Product Input, out string message)
        {
            if (_unit.Product.All().Where(x => x.Name == Input.Name && x.Id != Input.Id).Any())
            {
                message = "NameFoundBefore";
                return false;
            }
            message = "Done";
            return true;
        }
    }
}
