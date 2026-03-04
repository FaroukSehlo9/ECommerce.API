using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace ECommerce.Application.DTOS.AuthDTO
{
    public class JwtSettings
    {
        //public string Key { get; set; }
        //public string Issuer { get; set; }
        //public string Audience { get; set; }
        //public double DurationInMinutes { get; set; }
        private readonly IConfiguration _config;
        public JwtSettings(IConfiguration config)
        {
            _config = config;
            Key = _config["JwtSettings:Key"];
            Issuer = _config["JwtSettings:Issuer"];
            Audience = _config["JwtSettings:Audience"];
            ValidateIssuer = Convert.ToBoolean(_config["JwtSettings:validateIssuer"]);
            ValidateAudience = Convert.ToBoolean(_config["JwtSettings:validateAudience"]);
            ValidateLifeTime = Convert.ToBoolean(_config["JwtSettings:validateLifetime"]);
            ValidateIssuerSigningKey = Convert.ToBoolean(_config["JwtSettings:validateIssuerSigningKey"]);
            AccessTokenExpireDate = Convert.ToInt32(_config["JwtSettings:AccessTokenExpireDate"]);
            RefreshTokenExpireDate = Convert.ToInt32(_config["JwtSettings:RefreshTokenExpireDate"]);
        }



        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifeTime { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public int AccessTokenExpireDate { get; set; }
        public int RefreshTokenExpireDate { get; set; }
    }
}
