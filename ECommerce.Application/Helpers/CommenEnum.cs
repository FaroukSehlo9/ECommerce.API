using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Helpers
{
    public class CommenEnum
    {
        public enum RoleType
        {
            Admin =1,
            Manager =2,
            Customer =3
        }
        public enum OrderStatus
        {
            Pending = 1,
            Processing = 2,
            Shipped = 3,
            Delivered = 4,
            Cancelled = 5
        }
    }
}
