using NearExpiredProduct.Service.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Response
{
    public class CustomerResponse
    {
        [Key]
        public int Id { get; set; }
        [StringAttribute]
        public string Email { get; set; } = null!;
        [StringAttribute]
        public string CustomerPassword { get; set; } = null!;
        [StringAttribute]
        public string UserName { get; set; } = null!;
        [StringAttribute]
        public string Phone { get; set; } = null!;
        [StringAttribute]
        public string? Gender { get; set; }
        [DateRangeAttribute]
        public DateTime? DateOfBirth { get; set; }=null;
        [StringAttribute]
        public string? Address { get; set; }
        [StringAttribute]
        public string? Avatar { get; set; }
        public string? Fcmtoken { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string Token { get; set; }
    }
}
