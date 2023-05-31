using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class ResetPasswordRequest
    {
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? PasswordResetToken { get; set; }
    }
}
