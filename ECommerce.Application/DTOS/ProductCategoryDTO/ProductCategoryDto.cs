using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.ProductCategoryDTO
{
    public class ProductCategoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
