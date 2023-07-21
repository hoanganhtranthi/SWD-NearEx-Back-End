using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class CreateCampaginRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Exp { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public virtual CampaignDetailRequest CampaignDetail { get; set; }

    }
    public class CampaignDetailRequest
    {
        public DateTime DateApply { get; set; }
        public double PercentDiscount { get; set; }
        public int MinQuantity { get; set; }
    }
}
