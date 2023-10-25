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
    public class StoreResponse
    {
        [Key]
        public int Id { get; set; }
        [StringAttribute]
        public string? StoreName { get; set; }
        [StringAttribute]
        public string? Phone { get; set; }
        [StringAttribute]
        public string? Address { get; set; }
        [StringAttribute]
        public string? Logo { get; set; }
        public string Token { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string CoordinateString { get; set; }
    }
}
