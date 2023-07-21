using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class StoreRequest
    {
        public string? StoreName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Logo { get; set; }
        public string? CoordinateString { get; set; }
    }
    public class UpdateStoreRequest
    {
        public string? StoreName { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Logo { get; set; }
    }
    public class CreateStoreRequest
    {
        public string? StoreName { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Logo { get; set; }
        public string? CoordinateString { get; set; }
    }
}
