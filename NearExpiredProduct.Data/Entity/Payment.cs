using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string Method { get; set; } = null!;
        public int? Status { get; set; }
        public DateTime? Time { get; set; }
        public int? OrderId { get; set; }

        public virtual OrderOfCustomer? Order { get; set; }
    }
}
