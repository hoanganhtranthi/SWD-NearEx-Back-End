using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string CateName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
