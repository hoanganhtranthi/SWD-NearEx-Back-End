using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class CampaignDetail
    {
        public int Id { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public int? CampaignId { get; set; }

        public virtual Campaign? Campaign { get; set; }
    }
}
