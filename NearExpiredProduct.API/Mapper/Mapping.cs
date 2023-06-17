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
            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<ProductRequest, ProductResponse>().ReverseMap();

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<CategoryRequest, CategoryResponse>().ReverseMap();

            CreateMap<Store, StoreRequest>().ReverseMap();
            CreateMap<Store, StoreResponse>().ReverseMap();
            CreateMap<StoreRequest, StoreResponse>().ReverseMap();
        }

    }
}
