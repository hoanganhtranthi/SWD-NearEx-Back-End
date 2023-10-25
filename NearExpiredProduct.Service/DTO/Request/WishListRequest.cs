using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class WishListRequest
    {
        public int CustomerId { get; set; }
        public int CampaignId { get; set; }
    }
}
