using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class OrderFilterRequest
    {
        public int CampaignId { get; set; }
        public int Count { get; set; }
    }
    public class OrderAmoutByCateRequest
    {
        public string CateName { get; set; }
        public decimal Total { get; set; }
    }
}
