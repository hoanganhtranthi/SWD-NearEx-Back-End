using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class OrderProduct
    {
        public OrderProduct()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int Status { get; set; }
        public decimal TotalPrice { get; set; }
        public int? CustomerId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
