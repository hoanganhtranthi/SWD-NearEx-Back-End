using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Service.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NearExpiredProduct.Service.Helpers.Enum;

namespace NearExpiredProduct.Service.DTO.Response
{
    public class CampaignResponse
    {
        [Key]
        public int Id { get; set; }
        [DateRangeAttribute]
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; }=null;
        [IntAttribute]
        public int? Status { get; set; }
        [DateRangeAttribute]
        public DateTime? Exp { get; set; }=null ;
        [IntAttribute]
        public int? ProductId { get; set; }
        [IntAttribute]
        public int? Quantity { get; set; }
        public virtual ProductResponse? Product { get; set; }
        public virtual ICollection<CampaignDetailResponse> CampaignDetails { get; set; }
    }
    public class CampaignDetailResponse
    {
        public int Id { get; set; }
        public DateTime? DateApply { get; set; }
        public double? PercentDiscount { get; set; }
        public decimal? Discount { get; set; }
        public int? MinQuantity { get; set; }
    }
    }
