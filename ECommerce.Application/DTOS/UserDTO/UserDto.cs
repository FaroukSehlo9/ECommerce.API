using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.UserDTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }


        public string Phone { get; set; }

        public int Role { get; set; }
        public string? RoleType { get; set; }

    }
}
