using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class OrderRequest
    {
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public int? Quantity { get; set; }=null;
        public int? CampaignId { get; set; }
        public int? CustomerId { get; set; }
    }
    public class CreateOrderRequest
    {
        public DateTime? OrderDate { get; set; }
        public int? Quantity { get; set; } = null;
        public int? CampaignId { get; set; }
        public int? CustomerId { get; set; }
        public CreatePaymentRequest PaymentRequest { get; set; }
    }
    public class UpdateOrderRequest
    {
        public int? Status { get; set; }
    }
}
