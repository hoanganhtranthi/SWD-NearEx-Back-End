using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class CampaignAmoutByStoreRequest
    {
        public string StoreName { get; set; }
        public int AmoutCampaign { get; set; }
    }
}
