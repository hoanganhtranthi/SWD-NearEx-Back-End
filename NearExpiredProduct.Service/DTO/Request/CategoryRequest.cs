using NearExpiredProduct.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class CategoryRequest
    {
        public string? CategoryName { get; set; } = null!;
    }
}
