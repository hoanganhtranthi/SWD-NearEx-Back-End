using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class StoreRequest
    {
        public string StoreName { get; set; } = null!;
        public string? Address { get; set; }
        public string Phone { get; set; } = null!;
        public string StoreAccount { get; set; } = null!;
        public byte[] Password { get; set; } = null!;
        public string? Logo { get; set; }
    }
}
