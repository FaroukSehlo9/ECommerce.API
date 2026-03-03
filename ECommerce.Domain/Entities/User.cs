using ECommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class User : Auditable
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public int RoleId { get; set; }
        



    }
}
