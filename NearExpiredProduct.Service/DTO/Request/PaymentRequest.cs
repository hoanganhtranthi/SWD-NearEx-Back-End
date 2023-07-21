using NearExpiredProduct.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class PaymentRequest
    {
        public string Method { get; set; } = null!;
        public int? Status { get; set; }
        public DateTime? Time { get; set; }
        public int? OrderId { get; set; }
    }
    public class CreatePaymentRequest
    {
        public string Method { get; set; } = null!;
        public DateTime? Time { get; set; }
    }
}
