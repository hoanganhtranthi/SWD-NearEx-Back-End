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
        public decimal Price { get; set; }
        public string? Origin { get; set; }
        public string ProductImg { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public int UnitInStock { get; set; }
        public DateTime? Expiry { get; set; }
        public int CateId { get; set; }
        public int StoreId { get; set; }

        public virtual Category Cate { get; set; } = null!;
        public virtual Store Store { get; set; } = null!;
        public virtual ICollection<Campaign> Campaigns { get; set; }
    }
}
