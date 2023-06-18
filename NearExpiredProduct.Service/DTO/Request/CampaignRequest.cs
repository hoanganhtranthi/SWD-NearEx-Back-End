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
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EXP { get; set; }
        public int? Discount { get; set; }
        public int? ProductId { get; set; }
        public int Status { get; set; }

        public virtual Product? Product { get; set; }
    }
}
