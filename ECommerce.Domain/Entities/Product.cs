using ECommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Product : Auditable
    {
        [Required]
        public string Name { get; set; } 

        public string Description { get; set; } 

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        //user ID
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}
