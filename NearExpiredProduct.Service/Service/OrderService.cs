using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Data.UnitOfWork;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Exceptions;
using NearExpiredProduct.Service.Helpers;
using NearExpiredProduct.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static NearExpiredProduct.Service.Helpers.Enum;

namespace NearExpiredProduct.Service.Service
{
    public interface IOrderService
    {
        Task<PagedResults<OrderResponse>> GetOrders(OrderRequest request, PagingRequest paging);
        Task<OrderResponse> GetOrderById(int id);
        Task<PagedResults<OrderResponse>> GetOrderByStoreId(int storeId, PagingRequest paging);
        Task<OrderResponse> GetToUpdateOrderStatus(int id);
        Task<OrderResponse> UpdateOrder(int id, UpdateOrderRequest request);
        Task<OrderResponse> InsertOrder(CreateOrderRequest order);   
    }
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponse> GetOrderById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Order Invalid", "");
                }
                var response = await _unitOfWork.Repository<OrderOfCustomer>().GetAll().Where(a => a.Id == id).Select(a => new OrderResponse
                {
                    Id = a.Id,
                    CampaignId = a.CampaignId,
                    CustomerId = a.CustomerId,
                    OrderDate = a.OrderDate,
                    Customer = _mapper.Map<CustomerResponse>(a.Customer),
                    ProductImg = a.Campaign.Product.ProductImg,
                    ProductName = a.Campaign.Product.ProductName,
                    Quantity = a.Quantity,
                    Status = a.Status,
                    UnitPrice = (decimal)a.Campaign.CampaignDetails.OrderBy(x => x.DateApply).LastOrDefault(x => x.DateApply <= a.OrderDate).Discount * a.Quantity,
                    StoreName = _unitOfWork.Repository<Store>().GetAll().SingleOrDefault(x => x.Id == a.Campaign.Product.StoreId).StoreName,
                    Payments = new List<PaymentResponse>
                   (a.Payments.Select(x => new PaymentResponse
                   {
                       Id = x.Id,
                       Method = x.Method,
                       OrderId = x.OrderId,
                       Status = x.Status,
                       Time = x.Time,
                   }))
                }).SingleOrDefaultAsync();

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found order with id {id.ToString()}", "");
                }

                return response;

            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Order By ID Error!!!", ex.InnerException?.Message);
            }
        }
        public async Task<PagedResults<OrderResponse>> GetOrderByStoreId(int storeId, PagingRequest paging)
        {
            try
            {
                var categorys = await _unitOfWork.Repository<OrderOfCustomer>().GetAll().Where(a=>a.Campaign.Product.StoreId==storeId).Select(a => new OrderResponse
                {
                    Id = a.Id,
                    CampaignId = a.CampaignId,
                    CustomerId = a.CustomerId,
                    OrderDate = a.OrderDate,
                    Customer = _mapper.Map<CustomerResponse>(a.Customer),
                    ProductImg = a.Campaign.Product.ProductImg,
                    ProductName = a.Campaign.Product.ProductName,
                    Quantity = a.Quantity,
                    Status = a.Status,
                    UnitPrice = (decimal)a.Campaign.CampaignDetails.OrderBy(x => x.DateApply).LastOrDefault(x => x.DateApply <= a.OrderDate).Discount * a.Quantity,
                    StoreName = _unitOfWork.Repository<Store>().GetAll().SingleOrDefault(x => x.Id == a.Campaign.Product.StoreId).StoreName,
                    Payments = new List<PaymentResponse>
                   (a.Payments.Select(x => new PaymentResponse
                   {
                       Id = x.Id,
                       Method = x.Method,
                       OrderId = x.OrderId,
                       Status = x.Status,
                       Time = x.Time,
                   }))
                }).ToListAsync();
                var sort = PageHelper<OrderResponse>.Sorting(paging.SortType, categorys, paging.ColName);
                var result = PageHelper<OrderResponse>.Paging(sort, paging.Page, paging.PageSize);
                return result;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get order by storeId error!!!!!", ex.Message);
            }
        }

        public async Task<PagedResults<OrderResponse>> GetOrders(OrderRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<OrderResponse>(request);
                var categorys = await _unitOfWork.Repository<OrderOfCustomer>().GetAll().Select(a => new OrderResponse
                {
                    Id = a.Id,
                    CampaignId = a.CampaignId,
                    CustomerId = a.CustomerId,
                    OrderDate = a.OrderDate,
                    Customer = _mapper.Map<CustomerResponse>(a.Customer),
                    ProductImg = a.Campaign.Product.ProductImg,
                    ProductName = a.Campaign.Product.ProductName,
                    Quantity = a.Quantity,
                    Status = a.Status,
                    UnitPrice =(decimal)a.Campaign.CampaignDetails.OrderBy(x=>x.DateApply).LastOrDefault(x => x.DateApply <= a.OrderDate).Discount * a.Quantity,
                    StoreName = _unitOfWork.Repository<Store>().GetAll().SingleOrDefault(x => x.Id == a.Campaign.Product.StoreId).StoreName,
                    Payments = new List<PaymentResponse>
                   (a.Payments.Select(x => new PaymentResponse
                   {
                       Id = x.Id,
                       Method = x.Method,
                       OrderId = x.OrderId,
                       Status = x.Status,
                       Time = x.Time,
                   }))
                }).DynamicFilter(filter).ToListAsync();
                var sort = PageHelper<OrderResponse>.Sorting(paging.SortType, categorys, paging.ColName);
                var result = PageHelper<OrderResponse>.Paging(sort, paging.Page, paging.PageSize);
                return result;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get order list error!!!!!", ex.Message);
            }
        }

        public async Task<OrderResponse> GetToUpdateOrderStatus(int id)
        {
            try
            {
                var order = await _unitOfWork.Repository<OrderOfCustomer>().GetAll()
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
                order.Status = (int)OrderStatusEnum.Finish;
                foreach(var pay in order.Payments)
                {
                    if (pay.Method.Equals("COD"))
                    {
                        pay.Status = (int)PaymentEnum.Finish;
                        pay.Time = DateTime.Today;
                    }
                }

                await _unitOfWork.Repository<OrderOfCustomer>().Update(order,order.Id);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<OrderResponse>(order);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Error", e.Message);
            }
        }

        public async Task<OrderResponse> InsertOrder(CreateOrderRequest order)
        {
            try
            {
                if (order == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Order Invalid!!!", "");
                }

                var p = _mapper.Map<OrderOfCustomer>(order);
                var payment = _mapper.Map<Payment>(order.PaymentRequest);
                if (payment.Method != "COD") payment.Status = (int)PaymentEnum.Pending;
                else payment.Status = (int)PaymentEnum.Finish;
                p.Status = (int)OrderStatusEnum.Pending;
                p.Payments.Add(payment);
               await _unitOfWork.Repository<OrderOfCustomer>().CreateAsync(p);
                await _unitOfWork.CommitAsync();

                var camp =  _unitOfWork.Repository<Campaign>().GetAll().Include(a=>a.CampaignDetails).Where(a => a.Id == p.CampaignId).SingleOrDefault();
               var pro= _unitOfWork.Repository<Product>().GetAll().Where(a => a.Id == camp.ProductId).SingleOrDefault();
                camp.Quantity =(int)(camp.Quantity - order.Quantity);
                await _unitOfWork.Repository<Campaign>().Update(camp,camp.Id);
                await _unitOfWork.CommitAsync();

                var ord= _mapper.Map<OrderOfCustomer,OrderResponse>(p);
                ord.ProductName = pro.ProductName;
                ord.ProductImg = pro.ProductImg;
                ord.StoreName = _unitOfWork.Repository<Store>().GetAll().Where(a => a.Id == pro.StoreId).SingleOrDefault().StoreName;
                ord.UnitPrice = (decimal)camp.CampaignDetails.OrderBy(x => x.DateApply).LastOrDefault(x => x.DateApply <= order.OrderDate).Discount * (int)order.Quantity;
                return ord;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Product Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<OrderResponse> UpdateOrder(int id, UpdateOrderRequest request)
        {
            try
            {
                var ordRequest = await _unitOfWork.Repository<OrderOfCustomer>().GetAsync(u => u.Id == id);
                if (ordRequest == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found order with id {id.ToString()}", "");
                }

                ordRequest.Status = (int)request.Status;

                await _unitOfWork.Repository<OrderOfCustomer>().Update(ordRequest, ordRequest.Id);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<OrderResponse>(ordRequest);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Campaign Error!!!", ex.InnerException?.Message);
            }
        }
    }
    }
