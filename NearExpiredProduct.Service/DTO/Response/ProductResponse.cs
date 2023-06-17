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
        [IntAttribute]
        public decimal? Price { get; set; }=null;
        [StringAttribute]
        public string? Origin { get; set; }
        public string ProductImg { get; set; } = null!;
        [StringAttribute]
        public string ProductName { get; set; } = null!;
        [StringAttribute]
        public string? Description { get; set; }
        [StringAttribute]
        public string Unit { get; set; } = null!;
        [IntAttribute]
        public int? NetWeight { get; set; }=null!;
        [IntAttribute]
        public int? CategoryId { get; set; }=null!;
        [IntAttribute]
        public int? StoreId { get; set; } = null!;
    }
}
