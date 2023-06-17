using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }
        public int Quantity { get; set; }
        public int? Discount { get; set; }

        public virtual OrderProduct? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
