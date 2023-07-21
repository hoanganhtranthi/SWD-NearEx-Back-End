using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using System.Text;
using System.Threading.Tasks;
using static NearExpiredProduct.Service.Helpers.Enum;
using Twilio.Http;
using Twilio.TwiML.Voice;
using Task = System.Threading.Tasks.Task;
using static Google.Apis.Requests.BatchRequest;
using System.Net.NetworkInformation;
using NearExpiredProduct.Data.Extensions;
using static Google.Rpc.Context.AttributeContext.Types;

namespace NearExpiredProduct.Service.Service
{
    public interface ICampaignService
    {
        Task<PagedResults<CampaignResponse>> GetCampaigns(CampaignRequest request, PagingRequest paging);
        Task<CampaignResponse> GetCampaignById(int id);
        Task<CampaignResponse> GetToUpdateCampaignStatus(int id);
        Task<CampaignResponse> UpdateCampaign(int id, UpdateCampaignRequest request);
        Task<CampaignResponse> InsertCampaign(CreateCampaginRequest campagin);
        Task<List<CampaignAmoutByStoreRequest>> GetCountCampaignByStore(int cateId);
        Task<PagedResults<CampaignResponse>> GetCampaignByStore(int storeId, PagingRequest paging);
        Task<PagedResults<CampaignResponse>> GetCampaignByCategory(int cateId, PagingRequest paging);
        Task<CustomerResponse> AddWishList(WishListRequest request);
        Task<PagedResults<CampaignResponse>> GetWishList(int cusId, PagingRequest paging);
        Task<CustomerResponse> DeleteWishList(WishListRequest request);
        Task<PagedResults<CampaignResponse>> GetBestSellerCampaign(PagingRequest paging);
        Task<PagedResults<CampaignResponse>> GetFavoriteCampaign(PagingRequest paging);
    }
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper,ICacheService cacheService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cacheService=cacheService;
        }


        public async Task<CampaignResponse> GetCampaignById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Campaign Invalid", "");
                }
                var response = await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Id == id).Select(a => new CampaignResponse
                {
                    Id = a.Id,
                    EndDate = a.EndDate,
                    Exp = a.Exp,
                    ProductId = a.ProductId,
                    Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                    StartDate = a.StartDate,
                    Quantity=a.Quantity,
                    Status = a.Status,
                    CampaignDetails = new List<CampaignDetailResponse>
                    (a.CampaignDetails.Select(x => new CampaignDetailResponse
                    {
                        Id = x.Id,
                        Discount = x.Discount,
                        DateApply = x.DateApply,
                        PercentDiscount= Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                        MinQuantity = x.MinQuantity,


                    }))
                }).SingleOrDefaultAsync();

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found campaign with id {id.ToString()}","" );
                }
                return response;

            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Campaign By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<PagedResults<CampaignResponse>> GetCampaigns(CampaignRequest request, PagingRequest paging)
        {
            try
            {
                var cacheData = _cacheService.GetData<PagedResults<CampaignResponse>>("Campaigns");
                List<CampaignResponse> response = new List<CampaignResponse>();
                if (request.ProductName != "")
                {
                    response = await _unitOfWork.Repository<Campaign>().GetAll().Where(a => EF.Functions.Collate(a.Product.ProductName.ToLower(), "SQL_Latin1_General_CP1_CI_AI").Contains(request.ProductName.ToLower())).Select(a => new CampaignResponse
                    {
                        Id = a.Id,
                        EndDate = a.EndDate,
                        Exp = a.Exp,
                        ProductId = a.ProductId,
                        Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                        StartDate = a.StartDate,
                        Quantity = a.Quantity,
                        Status = a.Status,
                        CampaignDetails = new List<CampaignDetailResponse>
                       (a.CampaignDetails.Select(x => new CampaignDetailResponse
                       {
                           Id = x.Id,
                           Discount = x.Discount,
                           DateApply = x.DateApply,
                           PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                           MinQuantity = x.MinQuantity,


                       }))
                    }).ToListAsync();
                }
                else
                {
                    var filter = _mapper.Map<CampaignResponse>(request);
                    response = await _unitOfWork.Repository<Campaign>().GetAll().Select(a => new CampaignResponse
                    {
                        Id = a.Id,
                        EndDate = a.EndDate,
                        Exp = a.Exp,
                        ProductId = a.ProductId,
                        Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                        StartDate = a.StartDate,
                        Quantity = a.Quantity,
                        Status = a.Status,
                        CampaignDetails = new List<CampaignDetailResponse>
                           (a.CampaignDetails.Select(x => new CampaignDetailResponse
                           {
                               Id = x.Id,
                               Discount = x.Discount,
                               DateApply = x.DateApply,
                               PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                               MinQuantity = x.MinQuantity,
                           }))
                    }).DynamicFilter(filter).ToListAsync();
                }
                var sort = PageHelper<CampaignResponse>.Sorting(paging.SortType, response, paging.ColName);
                var result = PageHelper<CampaignResponse>.Paging(sort, paging.Page, paging.PageSize);
                if (cacheData == null)
                {
                    var expiryTime = DateTimeOffset.Now.AddMinutes(2);
                    cacheData = result;
                    _cacheService.SetData<PagedResults<CampaignResponse>>("Campaigns", cacheData, expiryTime);
                }
                return result;
            
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get campaign list error!!!!!", ex.Message);
            }
        }

        public async Task<CampaignResponse> InsertCampaign(CreateCampaginRequest campagin)
        {
            try
            {
                var cateRequest =  _unitOfWork.Repository<Campaign>().GetAll().SingleOrDefault(u => u.StartDate.Equals(campagin.StartDate) && u.ProductId == campagin.ProductId);
                if (campagin == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Campaign Invalid!!!", "");
                }
                if (cateRequest != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Campaign has already insert !!!", "");
                }
                var pro = await _unitOfWork.Repository<Product>().GetAsync(a => a.Id == campagin.ProductId);
                pro.Category = await _unitOfWork.Repository<Category>().GetAsync(a => a.Id == pro.CategoryId);

                Campaign cam = new Campaign();
                cam.StartDate = campagin.StartDate;
                cam.EndDate = campagin.EndDate;
                cam.Status = (int)CampaginStatusEnum.New;
                cam.Exp = campagin.Exp;
                cam.ProductId = campagin.ProductId;
                cam.Product = pro;
                cam.Quantity = campagin.Quantity;
   
                    CampaignDetail campaignDetail = new CampaignDetail();
                    campaignDetail.MinQuantity = campagin.CampaignDetail.MinQuantity;
                    campaignDetail.Discount = decimal.Parse(((100 - double.Parse(campagin.CampaignDetail.PercentDiscount.ToString())) / 100).ToString()) * pro.Price;
                campaignDetail.CampaignId = cam.Id;
                    campaignDetail.DateApply =(DateTime)campagin.CampaignDetail.DateApply;
                cam.CampaignDetails.Add(campaignDetail);
                   

                await _unitOfWork.Repository<Campaign>().CreateAsync(cam);
                await _unitOfWork.CommitAsync();

                var store = _unitOfWork.Repository<Store>().Find(a => a.Id == pro.StoreId);

                var campaignResult = await _unitOfWork.Repository<Campaign>().GetAll()
                       .Include(x => x.CampaignDetails).Where(a=>a.Id==cam.Id)
                       .Select(x => new CampaignResponse()
                       {
                           Id = x.Id,
                           EndDate = x.EndDate,
                           StartDate= x.StartDate,
                           ProductId=x.ProductId,
                           Exp=x.Exp,
                           Product=_mapper.Map<ProductResponse>(pro),
                           Quantity=x.Quantity,
                           Status=x.Status,
                            CampaignDetails =new List<CampaignDetailResponse>
                        (x.CampaignDetails.Select(x => new CampaignDetailResponse
                    {
                        Id = x.Id,
                        Discount = campaignDetail.Discount,
                        DateApply = x.DateApply,
                        PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / pro.Price) * 100))))),
                        MinQuantity = x.MinQuantity,


                    }))
                       })
                       .FirstOrDefaultAsync();
                return campaignResult;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Campaign Error!!!", ex.InnerException?.Message);
            }
        }


        public async Task<CampaignResponse> UpdateCampaign(int id, UpdateCampaignRequest request)
        {
            try
            {
                var camRequest = await _unitOfWork.Repository<Campaign>().GetAsync(u => u.Id == id);
                if (camRequest == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Campaign Invalid!!!", "");
                }
                if(request.createCampaignDetailRequest != null)
                {
                    if ( await _unitOfWork.Repository<CampaignDetail>().GetAll().Where(a => a.DateApply.Equals(request.createCampaignDetailRequest.DateApply) && a.CampaignId==id).SingleOrDefaultAsync() != null)
                    {
                        throw new CrudException(HttpStatusCode.BadRequest, "Campaign has already insert !!!", "");
                    }
                    var pro = _unitOfWork.Repository<Product>().Find(a => a.Id == camRequest.ProductId);
                pro.Category = await _unitOfWork.Repository<Category>().GetAsync(a => a.Id == pro.CategoryId);

                camRequest.Status = (int)CampaginStatusEnum.Avaliable;
                    CampaignDetail campaignDetail = new CampaignDetail();
                    campaignDetail.MinQuantity = request.createCampaignDetailRequest.MinQuantity;
                    campaignDetail.Discount = decimal.Parse(((100 - double.Parse(request.createCampaignDetailRequest.PercentDiscount.ToString())) / 100).ToString()) * pro.Price;
                    campaignDetail.DateApply =(DateTime)request.createCampaignDetailRequest.DateApply;
                camRequest.CampaignDetails.Add(campaignDetail);
                }
                _mapper.Map<UpdateCampaignRequest, Campaign>(request, camRequest);
                await _unitOfWork.Repository<Campaign>().Update(camRequest, camRequest.Id);
                await _unitOfWork.CommitAsync();
                var response = await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Id == id).Select(a => new CampaignResponse
                {
                    Id = a.Id,
                    EndDate = a.EndDate,
                    Exp = a.Exp,
                    ProductId = a.ProductId,
                    Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                    StartDate = a.StartDate,
                    Quantity = a.Quantity,
                    Status = a.Status,
                    CampaignDetails = new List<CampaignDetailResponse>
                    (a.CampaignDetails.Select(x => new CampaignDetailResponse
                    {
                        Id = x.Id,
                        Discount = x.Discount,
                        DateApply = x.DateApply,
                        PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                        MinQuantity = x.MinQuantity,


                    }))
                }).SingleOrDefaultAsync();
                return response;
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

        public async Task<CampaignResponse> GetToUpdateCampaignStatus(int id)
        {
            try
            {
                var campaign = await _unitOfWork.Repository<Campaign>().GetAll()
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
                campaign.Status = (int)CampaginStatusEnum.OutOfStock;

                await _unitOfWork.Repository<Campaign>().Update(campaign,campaign.Id);
                await _unitOfWork.CommitAsync();

                var response =await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Id == id).Select(a => new CampaignResponse
                {
                    Id = a.Id,
                    EndDate = a.EndDate,
                    Exp = a.Exp,
                    ProductId = a.ProductId,
                    Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c=>c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                    StartDate = a.StartDate,
                    Quantity = a.Quantity,
                    Status = a.Status,
                    CampaignDetails = new List<CampaignDetailResponse>
                    (a.CampaignDetails.Select(x => new CampaignDetailResponse
                    {
                        Id = x.Id,
                        Discount = x.Discount,
                        DateApply = x.DateApply,
                        PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                        MinQuantity = x.MinQuantity,


                    }))
                }).SingleOrDefaultAsync();
                return response;
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
        
        
      
        public async Task<List<CampaignAmoutByStoreRequest>> GetCountCampaignByStore(int cateId)
        {
            try
            {
                var response =await _unitOfWork.Repository<Campaign>().GetAll().Include(a=>a.Product).Include(a=>a.Product.Store).Where(a=>a.Status!=(int)CampaginStatusEnum.OutOfStock && a.Product.CategoryId==cateId).ToListAsync();              
                var result = new List<CampaignAmoutByStoreRequest>();
                foreach(var campagin in response)
                {
                    var count = new CampaignAmoutByStoreRequest();
                    var cam = result.Where(a => a.StoreName.Equals(campagin.Product.Store.StoreName)).SingleOrDefault();
                    if (cam != null) continue;
                    count.StoreName = campagin.Product.Store.StoreName;
                    count.AmoutCampaign = response.Where(a => a.Product.Store.StoreName.Equals(count.StoreName)).ToList().Distinct().Count();
                    result.Add(count);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Campaign By Store Error !!!", ex.InnerException?.Message);
            }
        }
        public async Task<PagedResults<CampaignResponse>> GetCampaignByStore(int storeId, PagingRequest paging)
        {
            try
            {
                var cacheData = _cacheService.GetData<PagedResults<CampaignResponse>>("CampaignsByStore");
                var response = await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Product.StoreId == storeId).Select(a => new CampaignResponse
                {
                    Id = a.Id,
                    EndDate = a.EndDate,
                    Exp = a.Exp,
                    ProductId = a.ProductId,
                    Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                    StartDate = a.StartDate,
                    Quantity = a.Quantity,
                    Status = a.Status,
                    CampaignDetails = new List<CampaignDetailResponse>
                      (a.CampaignDetails.Select(x => new CampaignDetailResponse
                      {
                          Id = x.Id,
                          Discount = x.Discount,
                          DateApply = x.DateApply,
                          PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                          MinQuantity = x.MinQuantity,


                      }))
                }).ToListAsync();
                var result = PageHelper<CampaignResponse>.Paging(response, paging.Page, paging.PageSize);
                if (cacheData == null)
                {
                    var expiryTime = DateTimeOffset.Now.AddMinutes(2);
                    cacheData = result;
                    _cacheService.SetData<PagedResults<CampaignResponse>>("CampaignsByStore", cacheData, expiryTime);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Campaign By Store Error !!!", ex.InnerException?.Message);
            }
        }
        public async Task<PagedResults<CampaignResponse>> GetCampaignByCategory(int cateId, PagingRequest paging)
        {
            try
            {
                var cacheData = _cacheService.GetData<PagedResults<CampaignResponse>>("CampaignsByCate");
                var response =await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Product.CategoryId == cateId).Select(a => new CampaignResponse
                {
                    Id = a.Id,
                    EndDate = a.EndDate,
                    Exp = a.Exp,
                    ProductId = a.ProductId,
                    Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                    StartDate = a.StartDate,
                    Quantity = a.Quantity,
                    Status = a.Status,
                    CampaignDetails = new List<CampaignDetailResponse>
                      (a.CampaignDetails.Select(x => new CampaignDetailResponse
                      {
                          Id = x.Id,
                          Discount = x.Discount,
                          DateApply = x.DateApply,
                          PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                          MinQuantity = x.MinQuantity,


                      }))
                }).ToListAsync();
                var result = PageHelper<CampaignResponse>.Paging(response, paging.Page, paging.PageSize);
                if (cacheData == null)
                {
                    var expiryTime = DateTimeOffset.Now.AddMinutes(2);
                    cacheData = result;
                    _cacheService.SetData<PagedResults<CampaignResponse>>("CampaignsByCate", cacheData, expiryTime);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Campaign By Category Error !!!", ex.InnerException?.Message);
            }
        }       
        public async Task<PagedResults<CampaignResponse>> GetBestSellerCampaign( PagingRequest paging)
        {
            try
            {
                var response = await _unitOfWork.Repository<OrderOfCustomer>().GetAll().ToListAsync();
                var result = new List<OrderFilterRequest>();
                foreach (var order in response)
                {
                    var count = new OrderFilterRequest();
                    var cam = result.Where(a => a.CampaignId==order.CampaignId).SingleOrDefault();
                    if (cam != null) continue;
                    count.CampaignId =(int)order.CampaignId;
                    count.Count = response.Where(a => a.CampaignId==count.CampaignId).ToList().Distinct().Count();
                    result.Add(count);
                }
                var orders = result.OrderByDescending(x=>x.Count);
                List<CampaignResponse> list = new List<CampaignResponse>();
                CampaignResponse rs = new CampaignResponse();
                foreach(var ord in orders)
                {
                    rs = await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Id==ord.CampaignId).Select(a => new CampaignResponse
                    {
                        Id = a.Id,
                        EndDate = a.EndDate,
                        Exp = a.Exp,
                        ProductId = a.ProductId,
                        Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                        StartDate = a.StartDate,
                        Quantity = a.Quantity,
                        Status = a.Status,
                        CampaignDetails = new List<CampaignDetailResponse>
                       (a.CampaignDetails.Select(x => new CampaignDetailResponse
                       {
                           Id = x.Id,
                           Discount = x.Discount,
                           DateApply = x.DateApply,
                           PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                           MinQuantity = x.MinQuantity,


                       }))
                    }).SingleOrDefaultAsync();
                    list.Add(rs);
                }
                var listCampaign = PageHelper<CampaignResponse>.Paging(list, paging.Page, paging.PageSize);
                return listCampaign;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Best Seller Campaign Error !!!", ex.InnerException?.Message);
            }
        }
        public async Task<CustomerResponse> AddWishList(WishListRequest request)
        {
            try
            {
                Customer customer = _unitOfWork.Repository<Customer>()
                         .Find(c => c.Id == request.CustomerId);
                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found customer with id", request.CustomerId.ToString());
                }
                if (customer.WishList != null)
                {
                    var s = customer.WishList.Split(' ').ToList();
                    foreach (var id in s)
                    {
                        if (id.Equals("") || id == null) continue;
                        else if (int.Parse(id) != request.CampaignId)
                        {
                            customer.WishList += request.CampaignId.ToString().Trim() + " ";
                        }
                    }
                }
                else customer.WishList = request.CampaignId.ToString().Trim() + " ";
               
                await _unitOfWork.Repository<Customer>().Update(customer, customer.Id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Customer, CustomerResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Add wishlist error!!!!!", ex.Message);
            }

        }
        public async Task<PagedResults<CampaignResponse>> GetWishList(int cusId, PagingRequest paging)
        {
            try
            {
                Customer customer = _unitOfWork.Repository<Customer>()
                         .Find(c => c.Id == cusId);
                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found customer with id", cusId.ToString());
                }
                List<CampaignResponse> list =new List<CampaignResponse>();
                if (customer.WishList != null)
                {
                    var s = customer.WishList.Split(' ').ToList();
                    foreach (var id in s)
                    {
                        if (id.Equals("") || id == null) continue;
                        var campaign = await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Id == int.Parse(id.Trim())).Select(a => new CampaignResponse
                        {
                            Id = a.Id,
                            EndDate = a.EndDate,
                            Exp = a.Exp,
                            ProductId = a.ProductId,
                            Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                            StartDate = a.StartDate,
                            Quantity = a.Quantity,
                            Status = a.Status,
                            CampaignDetails = new List<CampaignDetailResponse>
                           (a.CampaignDetails.Select(x => new CampaignDetailResponse
                           {
                               Id = x.Id,
                               Discount = x.Discount ,
                               DateApply = x.DateApply,
                               PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                               MinQuantity = x.MinQuantity,


                           }))
                        }).SingleOrDefaultAsync();
                        list.Add(campaign);
                    }
                }
                var result = PageHelper<CampaignResponse>.Paging(list, paging.Page, paging.PageSize);
                return result;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Add wishlist error!!!!!", ex.Message);
            }

        }
        public async Task<CustomerResponse> DeleteWishList(WishListRequest request)
        {
            try
            {
                Customer customer = _unitOfWork.Repository<Customer>()
                         .Find(c => c.Id == request.CustomerId);
                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found customer with id", request.CustomerId.ToString());
                }
                    var s = customer.WishList.Split(' ').ToList();
                    String wishList = null;
                    foreach (var id in s.ToList())
                    {
                        if (id.Equals("") || id == null) continue;
                        if (s.IndexOf(request.CampaignId.ToString()) >= 0 && int.Parse(id) == request.CampaignId)
                        {
                            s.Remove(request.CampaignId.ToString());
                        }
                        else wishList += id + " ";
                    }
                    customer.WishList = wishList;
                await _unitOfWork.Repository<Customer>().Update(customer, customer.Id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Customer, CustomerResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete wishlist error!!!!!", ex.Message);
            }
}

        public async Task<PagedResults<CampaignResponse>> GetFavoriteCampaign(PagingRequest paging)
        {
            try
            {
                var cacheData = _cacheService.GetData<PagedResults<CampaignResponse>>("FavouriteCampaigns");
                var wishList = _unitOfWork.Repository<Customer>().GetAll().Select(a => a.WishList).ToList();
                List<OrderFilterRequest> result = new List<OrderFilterRequest>();
                    foreach (var wish in wishList)
                    {
                        if(wish != null || int.Parse(wish) != 0)
                    { 
                        var s = wish.Split(' ').ToList();
                        foreach (var id in s.ToList())
                        {
                            if (id.Equals("") || id == null) continue;
                            else
                            {
                                var cam = result.Where(a => a.CampaignId == int.Parse(id)).SingleOrDefault();
                                if (result.Count > 0 && cam != null) cam.Count++;
                                else
                                {
                                    var count = new OrderFilterRequest();
                                    count.CampaignId = int.Parse(id);
                                    count.Count = 1;
                                    result.Add(count);

                                }

                            }
                        }
                    }
                }
                    var campaigns = result.OrderByDescending(x => x.Count);
                    List<CampaignResponse> list = new List<CampaignResponse>();
                    CampaignResponse rs = new CampaignResponse();
                    foreach (var cam in campaigns)
                    {
                        rs = await _unitOfWork.Repository<Campaign>().GetAll().Where(a => a.Id == cam.CampaignId).Select(a => new CampaignResponse
                        {
                            Id = a.Id,
                            EndDate = a.EndDate,
                            Exp = a.Exp,
                            ProductId = a.ProductId,
                            Product = _mapper.Map<Product, ProductResponse>(_unitOfWork.Repository<Product>().GetAll().Include(x => x.Store).Include(c => c.Category).SingleOrDefault(x => x.Id == a.ProductId)),
                            StartDate = a.StartDate,
                            Quantity = a.Quantity,
                            Status = a.Status,
                            CampaignDetails = new List<CampaignDetailResponse>
                           (a.CampaignDetails.Select(x => new CampaignDetailResponse
                           {
                               Id = x.Id,
                               Discount = x.Discount,
                               DateApply = x.DateApply,
                               PercentDiscount = Double.Parse((String.Format("{0:00.0}", (100 - (double)((x.Discount / a.Product.Price) * 100))))),
                               MinQuantity = x.MinQuantity,


                           }))
                        }).SingleOrDefaultAsync();
                        list.Add(rs);
                    }
                var listCampaign = PageHelper<CampaignResponse>.Paging(list, paging.Page, paging.PageSize);
                    if (cacheData == null)
                     {
                         var expiryTime = DateTimeOffset.Now.AddMinutes(2);
                           cacheData = listCampaign;
                         _cacheService.SetData<PagedResults<CampaignResponse>>("Campaigns", cacheData, expiryTime);
                      }
                return listCampaign;
                     }
                        catch (Exception ex)
                         {
                             throw new CrudException(HttpStatusCode.BadRequest, " Get Favourite Campaign Error !!!", ex.InnerException?.Message);
                          }  
}
    }
}
