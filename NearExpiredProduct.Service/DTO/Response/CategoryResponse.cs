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
    public class CategoryResponse
    {
        [Key]
        public int Id { get; set; }
        [StringAttribute]
        public string CateName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
