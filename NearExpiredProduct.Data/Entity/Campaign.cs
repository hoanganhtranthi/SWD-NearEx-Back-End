using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class Campaign
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public decimal MinPrice { get; set; }
        public int Quantity { get; set; }
        public int? Discount { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }

        public virtual OrderOfCustomer? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
