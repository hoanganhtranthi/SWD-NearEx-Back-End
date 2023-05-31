using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class OrderOfCustomer
    {
        public OrderOfCustomer()
        {
            Campaigns = new HashSet<Campaign>();
        }

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int Status { get; set; }
        public decimal TotalPrice { get; set; }
        public int? CustomerId { get; set; }
        public byte[]? PaymentMethod { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<Campaign> Campaigns { get; set; }
    }
}
