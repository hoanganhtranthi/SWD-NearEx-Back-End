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
        public string? Code { get; set; } = null!;
        public decimal? Price { get; set; }
        public string? Origin { get; set; }
        public string? ProductName { get; set; } = null!;
        public string? Unit { get; set; } = null!;
        public double? NetWeight { get; set; }
        public int? CategoryId { get; set; }
        public int? StoreId { get; set; }
        public int? Status { get; set; } = null;
    }
    public class CreateProductRequest
    {
        public string Code { get; set; } = null!;
        public decimal Price { get; set; }
        public string Origin { get; set; }
        public string ProductImg { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string Description { get; set; }
        public string Unit { get; set; } = null!;
        public double NetWeight { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }
    }
    public class UpdateProductRequest
    {
        public decimal Price { get; set; }
        public string? Origin { get; set; }
        public string ProductImg { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Unit { get; set; } = null!;
        public double? NetWeight { get; set; }
        public int? CategoryId { get; set; }
        public int StoreId { get; set; }
    }
}
