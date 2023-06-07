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
    public class ProductResponse
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public decimal Price { get; set; }
        [StringAttribute]
        public string? Origin { get; set; }
        public string ProductImg { get; set; } = null!;
        [StringAttribute]
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public int UnitInStock { get; set; }
        [DateRangeAttribute]
        public DateTime? Expiry { get; set; }
        [Key]
        public int CateId { get; set; }
        [Key]
        public int StoreId { get; set; }

        public virtual Category Cate { get; set; } = null!;
        public virtual Store Store { get; set; } = null!;
        public virtual ICollection<Campaign> Campaigns { get; set; }
    }
}
