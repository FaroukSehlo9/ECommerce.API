using AutoMapper;
using ECommerce.API.Utilities;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.AuthDTO;
using ECommerce.Application.Helpers;
using ECommerce.Application.IService;
using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static ECommerce.Application.Helpers.CommenEnum;

namespace ECommerce.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unit;
        private readonly IStringLocalizer<GeneralMessages> _localization;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unit, IStringLocalizer<GeneralMessages> localization, IConfiguration config, IMapper Mapper
            , IUserService userService)
        {
            _unit = unit;
            _localization = localization;
            _config = config;
            _mapper = Mapper;
            _userService = userService;
            
        }



        public async Task<GeneralResponse<AuthResponse>> Login(AuthRequest request)
        {
            #region CheckIsExist
            var Pass = WebUiUtility.Encrypt(request.Password);
            var User = _unit.User.All().Where(x => x.Email == request.Email && x.PasswordHash == Pass ).FirstOrDefault();
            if (User == null)
            {

                return new GeneralResponse<AuthResponse>(_localization["UserNotFound"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            #endregion
           

            #region Generate JWT
            JwtSecurityToken jwtSecurityToken = await GenerateToken(User, DateTime.UtcNow.AddYears(3));
            AuthResponse response = new AuthResponse
            {
                UserId = User.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = User.Email,
                UserName = User.UserName,
                RoleId = User.Role
            };

            #endregion
            return new GeneralResponse<AuthResponse>(response, _localization["LoginSuccessfully"].Value, 0);

        }

        public async Task<GeneralResponse<RegistrationResponse>> Register(RegistrationRequest request)
        {
            try
            {

                #region ValidEmail


                //mapping
                User User = _mapper.Map<RegistrationRequest, User>(request);
                if (!_userService.CheckUser(User, out string message))
                {
                    return new GeneralResponse<RegistrationResponse>(_localization[message], System.Net.HttpStatusCode.BadRequest);

                }
                #endregion
                //Mapping
                User.Role = Convert.ToInt32(RoleType.Customer);
                User.PasswordHash = WebUiUtility.Encrypt(request.PasswordHash);
           
                await _unit.User.AddAsync(User);
                var results = _unit.Save();







                #region Token

                //JwtSecurityToken jwtSecurityToken = await GenerateToken(User, DateTime.UtcNow.AddDays(2));

                RegistrationResponse response = new RegistrationResponse
                {
                    UserId = User.Id,
                    Email = User.Email,
                    UserName = User.UserName
                };

                #endregion

                return results >= 1 ?
             new GeneralResponse<RegistrationResponse>(response, _localization["RegistrationSuccessfully"], 0)
             : new GeneralResponse<RegistrationResponse>(_localization["ErrorInRegistration"], System.Net.HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                return new GeneralResponse<RegistrationResponse>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }


        private async Task<JwtSecurityToken> GenerateToken(User user, DateTime expiredDate)
        {
            var claims = new[]
               {
                        new Claim("user_id", user.Id.ToString()),
                        new Claim("role",user.Role.ToString()),
                        new Claim("Email", user.Email),
                 };



            var _jwtSettings = new JwtSettings(_config);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiredDate,
                //expires: expiredDate.AddYears(2),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature));
            return jwtSecurityToken;
        }
    }
}
