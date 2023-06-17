using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class Store
    {
        public Store()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string StoreName { get; set; } = null!;
        public string? Address { get; set; }
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Logo { get; set; }
        public string? Fcmtoken { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
