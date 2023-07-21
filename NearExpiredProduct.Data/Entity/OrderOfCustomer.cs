using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class OrderOfCustomer
    {
        public OrderOfCustomer()
        {
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public int Quantity { get; set; }
        public int? CampaignId { get; set; }
        public int? CustomerId { get; set; }

        public virtual Campaign? Campaign { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
