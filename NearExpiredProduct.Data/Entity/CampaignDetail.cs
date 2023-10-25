using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class CampaignDetail
    {
        public int Id { get; set; }
        public decimal Discount { get; set; }
        public int? MinQuantity { get; set; }
        public DateTime DateApply { get; set; }
        public int? CampaignId { get; set; }

        public virtual Campaign? Campaign { get; set; }
    }
}
