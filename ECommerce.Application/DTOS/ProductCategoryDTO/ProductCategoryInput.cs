using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.ProductCategoryDTO
{
    public class ProductCategoryInput
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
