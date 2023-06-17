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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Status { get; set; }
        public int? Discount { get; set; }
        public DateTime Exp { get; set; }
        public int? ProductId { get; set; }
        public virtual ICollection<CampaignDetailRequest> CampaignDetails { get; set; }

    }
    public class CampaignDetailRequest
    {
        public decimal? UnitPrice { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
    }
}
