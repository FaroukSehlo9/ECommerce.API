using AutoMapper;
using ECommerce.API.Utilities;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.UserDTO;
using ECommerce.Application.IService;
using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static ECommerce.Application.Helpers.CommenEnum;

namespace ECommerce.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IMapper _Imapper;
        private readonly IStringLocalizer<GeneralMessages> _localization;
        private readonly IUnitOfWork _unit;

        public UserService(IMapper imapper, IStringLocalizer<GeneralMessages> localization, IUnitOfWork unitOfWork)
        {
            _Imapper = imapper;
            _localization = localization;
            _unit = unitOfWork;
        }
        public async Task<GeneralResponse<Guid>> Add(UserInput Input, Guid UserId)
        {
            try
            {
                var result = 0;
                var User = _Imapper.Map<UserInput, User>(Input);
                //User.CreatedBy = UserId;
                User.CreationDate = DateTime.Now;
                User.PasswordHash = WebUiUtility.Encrypt(Input.PasswordHash);

                #region CheckValid
                if (!CheckUser(User, out String message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                #endregion

                #region Add
                await _unit.User.AddAsync(User);
                #endregion
            
                return result >= 1 ? new GeneralResponse<Guid>(User.Id, _localization["AddedSuccesfully"].Value)
            : new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }
        public bool CheckEmail(string Email, Guid? id)
        {

            if (_unit.User.All().Where(x => (!String.IsNullOrEmpty(Email) && x.Email.Equals(Email)) && (id == null || id == Guid.Empty || id != x.Id)).FirstOrDefault() != null)

            { return true; }
            return false;
        }
    

        public bool Checkphone(string phone, Guid? id)
        {

            if (_unit.User.All().Where(x => (!String.IsNullOrEmpty(phone) && x.Phone.Equals(phone)) && (id == null || id == Guid.Empty || id != x.Id)).FirstOrDefault() != null)

            { return true; }
            else if (!WebUiUtility.ValidPhone(phone))
            { return true; }
            return false;
        }
        public bool CheckUser(User User, out string message)
        {
            if (CheckEmail(User.Email, User.Id) && Checkphone(User.Phone, User.Id))
            {
                message = "Email&Phone";
                return false;
            }
            else if (CheckEmail(User.Email, User.Id))
            {
                message = "EmailAlreadyExists";
                return false;
            }
            else if (Checkphone(User.Phone, User.Id) )
            {
                message = "PhoneNumberIsNotValid";
                return false;
            }
            message = "";
            return true;
        }

        public async Task<GeneralResponse<List<UserDto>>> GetAll()
        {
            var result = _unit.User.All().ToList().Select(x => new UserDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                Phone = x.Phone,
                Role = x.Role,
                RoleType = x.Role != null ? Enum.GetName(typeof(RoleType), x.Role) : null,
                PasswordHash = x.PasswordHash


            }).ToList();
            return new GeneralResponse<List<UserDto>>(result, _localization["Succes"].Value, result.Count());
        }

        public async Task<GeneralResponse<UserDto>> GetByIdAsync(Guid Id)
        {
            var result = _unit.User.All().Where(d => d.Id == Id).ToList().Select(x => new UserDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                Phone = x.Phone,
                Role = x.Role,
                RoleType = x.Role != null ? Enum.GetName(typeof(RoleType), x.Role) : null,
                PasswordHash = x.PasswordHash

            }).FirstOrDefault();
            return new GeneralResponse<UserDto>(result, _localization["Succes"].Value);
        }
        public async Task<GeneralResponse<List<AdminDto>>> GetAdmin()
        {
            var result = _unit.User.All().Where(x => x.Role == 1).ToList().Select(x => new AdminDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
            }).ToList();
            return new GeneralResponse<List<AdminDto>>(result, _localization["Succes"].Value, result.Count());
        }

        public User GetUserById(Guid UserID)
        {
            var User = _unit.User.All().Where(ex => ex.Id == UserID).FirstOrDefault();
            return User;
        }

        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {
            await _unit.User.SoftDelete(Id);
            var results = await _unit.SaveAsync();

            return results >= 1 ? new GeneralResponse<Guid>(Id, _localization["DeletedSuccesfully"].Value) :
                new GeneralResponse<Guid>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);

        }

        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {
            await _unit.User.SoftDeleteRangeAsync(Id);

            var results = _unit.Save();

            return results >= 1 ? new GeneralResponse<List<Guid>>(Id, _localization["DeletedSuccesfully"].Value) :
                 new GeneralResponse<List<Guid>>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);

        }

        public async Task<GeneralResponse<Guid>> Update(UserUpdateInput Input, Guid UserId)
        {
            try
            {

                User OldUser = await _unit.User.GetByIdAsync(Input.Id);
                var User = _Imapper.Map<UserUpdateInput, User>(Input, OldUser);
                User.UpdatedBy = UserId;
                User.UpdatedDate = DateTime.Now;

                #region CheckValid
                if (!CheckUser(User, out String message))
                {
                    return new GeneralResponse<Guid>(_localization[message], System.Net.HttpStatusCode.BadRequest);
                }
                #endregion

                await _unit.User.UpdateAsync(User);
              
                var result = _unit.Save();

                return result >= 1 ? new GeneralResponse<Guid>(User.Id, _localization["UpdatedSuccesfully"].Value)
                : new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }
    }

}
