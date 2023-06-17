using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class Campaign
    {
        public Campaign()
        {
            CampaignDetails = new HashSet<CampaignDetail>();
            OrderOfCustomers = new HashSet<OrderOfCustomer>();
        }

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Status { get; set; }
        public int? Discount { get; set; }
        public DateTime Exp { get; set; }
        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ICollection<CampaignDetail> CampaignDetails { get; set; }
        public virtual ICollection<OrderOfCustomer> OrderOfCustomers { get; set; }
    }
}
