using NearExpiredProduct.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class CampaignRequest
    {
        public DateTime? StartDate { get; set; }
        public int? Status { get; set; }
        public DateTime? Exp { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = "";
    }
    public class UpdateCampaignRequest
    {
        public DateTime? EndDate { get; set; } = null;
        public int? Quantity { get; set; }
        public CreateCampaignDetailRequest? createCampaignDetailRequest { get; set; }
    }
}
