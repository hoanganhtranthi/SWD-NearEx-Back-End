using NearExpiredProduct.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class CustomerRequest
    {
        public string? Email { get; set; }
        public string? CustomerPassword { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; } = null;
        public string? Address { get; set; }
        public string? Avatar { get; set; }
    }
}
