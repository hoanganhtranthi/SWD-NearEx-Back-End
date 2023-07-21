using NearExpiredProduct.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Response
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public string Method { get; set; } = null!;
        public int? Status { get; set; }
        public DateTime? Time { get; set; }
        public int? OrderId { get; set; }
    }
}
