using ECommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Category :Auditable
    {
        public string Name { get; set; }
        public string Description { get; set; } 

        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
