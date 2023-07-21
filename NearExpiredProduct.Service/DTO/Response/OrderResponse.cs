using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Service.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Response
{
    public class OrderResponse
    {
        [Key]
        public int Id { get; set; }
        [DateRangeAttribute]
        public DateTime? OrderDate { get; set; }=null;
        [IntAttribute]
        public int? Status { get; set; }
        [IntAttribute]
        public int? Quantity { get; set; }
        [IntAttribute]
        public int? CampaignId { get; set; }
        [IntAttribute]
        public int? CustomerId { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ProductImg { get; set; }
        public string StoreName { get; set; }
        public virtual CustomerResponse? Customer { get; set; }
        public virtual ICollection<PaymentResponse> Payments { get; set; }
    }
}
