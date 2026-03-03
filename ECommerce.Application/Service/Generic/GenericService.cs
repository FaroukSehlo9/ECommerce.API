using AutoMapper;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.IService.IGeneric;
using ECommerce.Domain.Entities.Base;
using ECommerce.Domain.IRepositories;
using ECommerce.Domain.IRepositories.Base;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service.Generic
{
    public class GenericService<T, DTO, Input, UpdateInput, Spec> : IGenericService<T, DTO, Input, UpdateInput, Spec> where T : Auditable where DTO : class where Input : class where UpdateInput : class where Spec : class
    {
        private readonly IUnitOfWork _unit;
        private readonly IRepository<T> _Repo;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GeneralMessages> _localizer;



        public GenericService(IUnitOfWork unitOfWork, IStringLocalizer<GeneralMessages> localizer, IRepository<T> Repo, IMapper mapper)
        {
            _unit = unitOfWork;
            _localizer = localizer;
            _Repo = Repo;
            _mapper = mapper;
        }
        public virtual async Task<GeneralResponse<Guid>> Add(Input input, string? TypeInt = null)
        {

            try
            {
                var obj = _mapper.Map<Input, T>(input);
                if (!valid(obj, out string message))
                {
                    return new GeneralResponse<Guid>(_localizer[message], System.Net.HttpStatusCode.BadRequest);
                }
                var entity = await _Repo.AddAsync(obj);
                var result = _unit.Save();
                Guid id = new Guid();
                PropertyInfo ImageBase64 = input.GetType().GetProperty("ImageBase64");
                PropertyInfo FileName = input.GetType().GetProperty("FileName");
                if (result > 0)
                {
                    object idValue = obj.GetType().GetProperty("Id").GetValue(entity, null);

                    // If 'Id' property is of type int, for example, you can cast it accordingly
                    id = (Guid)idValue;
                   
                }


                return result > 0 ? new GeneralResponse<Guid>(id, _localizer["succesfully"].Value)
                     : new GeneralResponse<Guid>(_localizer["errorinsave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }

  
        public virtual async Task<GeneralResponse<List<DTO>>> GetAll(List<string> includes)
        {
            //var spec = new DoctorSpecification(Params);
            //var Data2 = _Repo.GetAllWithSpecAsync((ISpecification<T>)spec).Result.ToList();

            var Data = await _Repo.GetAllAsync(includes);

            var result = _mapper.Map<List<T>, List<DTO>>(Data);

            return new GeneralResponse<List<DTO>>(result, _localizer["Succes"], result.Count());

        }

        public virtual async Task<GeneralResponse<DTO>> GetByIdAsync(Guid Id)
        {
            // var Data = _Repo.GetById(Id, includes).Result;

            Type type = typeof(Spec);
            var spec = (Spec)Activator.CreateInstance(type, Id);


            var Data = _Repo.GetByIdWithSpecAsync((ISpecification<T>)spec).Result;
            var result = _mapper.Map<T, DTO>(Data);
            //#region Get IMG
            //if (!String.IsNullOrEmpty(Params.ImageType))
            //{

            //    if ((Guid)idValue != null)
            //    {
            //        var ImageString = _documentService.GetMainImagePath((Guid)idValue, Params.ImageType);
            //        obj.GetType().GetProperty("Id").SetValue(obj, ImageString);

            //    }
            //}
            //#endregion
            return new GeneralResponse<DTO>(result, _localizer["Succes"]);
        }

        public virtual async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {
            await _Repo.SoftDelete(Id);
            var results = await _unit.SaveAsync();

            return results >= 1 ? new GeneralResponse<Guid>(Id, _localizer["DeletedSuccesfully"]) :
                new GeneralResponse<Guid>(_localizer["ErrorInDelete"], System.Net.HttpStatusCode.BadRequest);
        }

        public virtual async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {
            await _Repo.SoftDeleteRangeAsync(Id);

            var results = _unit.Save();

            return results >= 1 ? new GeneralResponse<List<Guid>>(Id, _localizer["DeletedSuccesfully"].Value) :
                 new GeneralResponse<List<Guid>>(_localizer["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);

        }

        public virtual async Task<GeneralResponse<Guid>> Update(UpdateInput input, string? TypeInt = null)
        {
            try
            {
                object idValue = input.GetType().GetProperty("Id").GetValue(input, null);
                PropertyInfo ImageBase64 = input.GetType().GetProperty("ImageBase64");
                PropertyInfo FileName = input.GetType().GetProperty("FileName");
                Guid id = (Guid)idValue;
                T OldObject = await _Repo.GetByIdAsync(id);
                var NewObj = _mapper.Map<UpdateInput, T>(input, OldObject);
                if (!valid(NewObj, out string message))
                {
                    return new GeneralResponse<Guid>(_localizer[message], System.Net.HttpStatusCode.BadRequest);
                }
                await _Repo.UpdateAsync(NewObj);
                var result = _unit.Save();
              
                return result >= 1 ? new GeneralResponse<Guid>(id, _localizer["UpdatedSuccesfully"])
                   : new GeneralResponse<Guid>(_localizer["ErrorInSave"], System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }
        public bool valid(T Obj, out string message)
        {
            Type type = typeof(Spec);
            string SpecName = type.Name;
            var spec = (Spec)Activator.CreateInstance(type, Obj);


            var DataExist = _Repo.GetAllWithSpecAsync((ISpecification<T>)spec).Result.ToList().Count != 0;
            if (DataExist)
            {
                message = SpecName.Replace("Specification", "") + "ExistBefore";
                return false;
            }
            message = "Done";
            return true;
        }
    }
}
