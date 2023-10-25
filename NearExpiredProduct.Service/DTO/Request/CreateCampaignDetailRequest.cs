using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class CreateCampaignDetailRequest
    {
        public DateTime? DateApply { get; set; }
        public double? PercentDiscount { get; set; }
        public int? MinQuantity { get; set; }
    }
}
