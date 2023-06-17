using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Service.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Response
{
    public class CampaignResponse
    {
        [Key]
        public int Id { get; set; }
        [DateRangeAttribute]
        public DateTime StartDate { get; set; }
        [DateRangeAttribute]
        public DateTime EndDate { get; set; }
        [IntAttribute]
        public int? Status { get; set; }
        [IntAttribute]
        public int? Discount { get; set; }
        [DateRangeAttribute]
        public DateTime Exp { get; set; }
        [IntAttribute]
        public int? ProductId { get; set; }

        public virtual ProductResponse? Product { get; set; }
        public virtual ICollection<CampaignDetailResponse> CampaignDetails { get; set; }
    }
    public class CampaignDetailResponse
    {
        public int Id { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
    }
    }
