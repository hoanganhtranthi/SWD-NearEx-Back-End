using NearExpiredProduct.Service.Commons;
using NetTopologySuite.Geometries;
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
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
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
        [StringAttribute]
        public string? GoogleId { get; set; }
        public string? Fcmtoken { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string Token { get; set; }
        [StringAttribute]
        public string CoordinateString { get; set; }
        public string? WishList { get; set; }
    }
}
