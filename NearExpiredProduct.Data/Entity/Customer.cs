using System;
using System.Collections.Generic;

namespace NearExpiredProduct.Data.Entity
{
    public partial class Customer
    {
        public Customer()
        {
            OrderOfCustomers = new HashSet<OrderOfCustomer>();
        }

        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? Fcmtoken { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public virtual ICollection<OrderOfCustomer> OrderOfCustomers { get; set; }
    }
}
