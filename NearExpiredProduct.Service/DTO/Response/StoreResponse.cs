using NearExpiredProduct.Service.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Response
{
    public class StoreResponse
    {
        [Key]
        public int Id { get; set; }
        [StringAttribute]
        public string StoreName { get; set; } = null!;
        [StringAttribute]
        public string Phone { get; set; } = null!;
        [StringAttribute]
        public string? Address { get; set; }
        public string? Logo { get; set; }
        public string Token { get; set; }
    }
}
