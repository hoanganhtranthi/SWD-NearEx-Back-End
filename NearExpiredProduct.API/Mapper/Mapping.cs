using AutoMapper;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusinessTier.Mapper
{
    public class Mapping : Profile
    {
        public Mapping() {
            CreateMap<Customer, CustomerResponse>().ReverseMap();
            CreateMap<Customer, CustomerRequest>().ReverseMap();
            CreateMap<CustomerResponse, CustomerRequest>().ReverseMap();
        }

    }
}
