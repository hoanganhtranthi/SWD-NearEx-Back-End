using NearExpiredProduct.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class ProductRequest
    {
        public decimal? Price { get; set; }
        public string? Origin { get; set; }
        public string? ProductImg { get; set; } = null!;
        public string? ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Unit { get; set; } = null!;
        public int? NetWeight { get; set; }
        public int? CategoryId { get; set; }
        public int? StoreId { get; set; }

    }
}
