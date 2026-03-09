using AutoMapper;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CategoryDTO;
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
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _Imapper;
        private readonly IStringLocalizer<GeneralMessages> _localization;
        private readonly IUnitOfWork _unit;
        public CategoryService(IMapper imapper, IStringLocalizer<GeneralMessages> localization, IUnitOfWork unitOfWork)
        {
            _Imapper = imapper;
            _localization = localization;
            _unit = unitOfWork;
        }

        public async Task<GeneralResponse<List<CategoryDto>>> GetAll()
        {

            var result = _unit.Category.All().ToList().Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description

            }).ToList();
            return new GeneralResponse<List<CategoryDto>>(result, _localization["Succes"].Value, result.Count());
        }

        public async Task<GeneralResponse<CategoryDto>> GetByIdAsync(Guid Id)
        {
            var result = _unit.Category.All().Where(d => d.Id == Id).ToList().Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description

            }).FirstOrDefault();
            return new GeneralResponse<CategoryDto>(result, _localization["Succes"].Value);
        }

        public async Task<GeneralResponse<Guid>> Add(CategoryInput Input, Guid UserId)
        {
            try
            {
                var Category = _Imapper.Map<CategoryInput, Category>(Input);
                Category.CreatedBy = UserId;
                Category.CreationDate = DateTime.Now;
                if (!ValidCategory(Category, out string message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                await _unit.Category.AddAsync(Category);
                var result = _unit.Save();
                return result > 0 ? new GeneralResponse<Guid>(Category.Id, _localization["succesfully"].Value)
                     : new GeneralResponse<Guid>(_localization["errorinsave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }

        public async Task<GeneralResponse<Guid>> Update(CategoryUpdateInput Input, Guid UserId)
        {
            try
            {
                Category OldCategory = await _unit.Category.GetByIdAsync(Input.Id);
                var Category = _Imapper.Map<CategoryUpdateInput, Category>(Input, OldCategory);
                Category.UpdatedBy = UserId;
                Category.UpdatedDate = DateTime.Now;
                if (!ValidCategory(Category, out string message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                await _unit.Category.UpdateAsync(Category);
                var result = _unit.Save();
                return result >= 1 ? new GeneralResponse<Guid>(Category.Id, _localization["UpdatedSuccesfully"].Value)
                   : new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {
            await _unit.Category.SoftDelete(Id);
            var results = await _unit.SaveAsync();

            return results >= 1 ? new GeneralResponse<Guid>(Id, _localization["DeletedSuccesfully"].Value) :
                new GeneralResponse<Guid>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);
        }

        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {
            await _unit.Category.SoftDeleteRangeAsync(Id);

            var results = _unit.Save();

            return results >= 1 ? new GeneralResponse<List<Guid>>(Id, _localization["DeletedSuccesfully"].Value) :
                 new GeneralResponse<List<Guid>>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);

        }

        public bool ValidCategory(Category Input, out string message)
        {
            if (_unit.Category.All().Where(x => x.Name == Input.Name && x.Id != Input.Id).Any())
            {
                message = "NameFoundBefore";
                return false;
            }
            message = "Done";
            return true;
        }
    }
}

