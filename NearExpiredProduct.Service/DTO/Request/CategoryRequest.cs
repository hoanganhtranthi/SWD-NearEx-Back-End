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
        public int Id { get; set; }
        public string CateName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
