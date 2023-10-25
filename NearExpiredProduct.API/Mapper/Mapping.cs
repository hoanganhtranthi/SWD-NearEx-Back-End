using AutoMapper;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NearExpiredProduct.API.Mapper
{
    public class Mapping : Profile
    {
        public Mapping() {
            CreateMap<Customer, CustomerResponse>().ReverseMap();
            CreateMap<Customer, CustomerRequest>().ReverseMap();
            CreateMap<UpdateCustomerRequest, Customer>().ReverseMap();
            CreateMap<CreateCustomerRequest,Customer>().ReverseMap();
            CreateMap<CustomerResponse, CustomerRequest>().ReverseMap();

            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<ProductRequest, ProductResponse>().ReverseMap();
            CreateMap<UpdateProductRequest, Product>().ReverseMap();
            CreateMap<CreateProductRequest, Product>().ReverseMap();

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<CategoryRequest, CategoryResponse>().ReverseMap();
            CreateMap<CreateCategoryRequest, Category>().ReverseMap();

            CreateMap<Store, StoreRequest>().ReverseMap();
            CreateMap<Store, StoreResponse>().ReverseMap();
            CreateMap<StoreRequest, StoreResponse>().ReverseMap();
            CreateMap<CreateStoreRequest, Store>().ReverseMap();
            CreateMap<UpdateStoreRequest,Store>().ReverseMap(); 

            CreateMap<Campaign, CampaignRequest>().ReverseMap();
            CreateMap<Campaign, CampaignResponse>().ReverseMap();
            CreateMap<CampaignRequest, CampaignResponse>().ReverseMap();
            CreateMap<CampaignDetail, CampaignDetailResponse>().ReverseMap();
            CreateMap<UpdateCampaignRequest, Campaign>().ReverseMap();

            CreateMap<OrderOfCustomer, OrderRequest>().ReverseMap();
            CreateMap<OrderOfCustomer, OrderResponse>().ReverseMap();
            CreateMap<OrderRequest, OrderResponse>().ReverseMap();
            CreateMap<CreateOrderRequest,OrderOfCustomer>().ReverseMap();

            CreateMap<Payment, PaymentRequest>().ReverseMap();
            CreateMap <Payment, PaymentResponse>().ReverseMap();
            CreateMap<Payment, CreatePaymentRequest>().ReverseMap();
            CreateMap<PaymentRequest, PaymentResponse>().ReverseMap();
        }

    }
}
