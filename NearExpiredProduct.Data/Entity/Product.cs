using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class Product
    {
        public Product()
        {
            Campaigns = new HashSet<Campaign>();
        }

        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Origin { get; set; }
        public string ProductImg { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public string Unit { get; set; } = null!;
        public double NetWeight { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }
        public int? Status { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Store Store { get; set; } = null!;
        public virtual ICollection<Campaign> Campaigns { get; set; }
    }
}
