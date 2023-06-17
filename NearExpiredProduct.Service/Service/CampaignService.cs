using AutoMapper;
using AutoMapper.QueryableExtensions;
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

namespace NearExpiredProduct.Service.Service
{
    public interface ICampaignService
    {
        Task<PagedResults<CampaignResponse>> GetCampaigns(CampaignRequest request, PagingRequest paging);
        Task<CampaignResponse> DeleteCampaign(int id);
        Task<CampaignResponse> GetCampaignById(int id);
        Task<CampaignResponse> UpdateCampaign(int id, CampaignRequest request);
        Task<CampaignResponse> InsertCampaign(CampaignRequest category);
    }
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Task<CampaignResponse> DeleteCampaign(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CampaignResponse> GetCampaignById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Campaign Invalid", "");
                }
                var response = _unitOfWork.Repository<Campaign>().GetAll().Where(a=>a.Id==id).Select(a => new CampaignResponse
                {
                    Id = a.Id,
                    Discount = a.Discount,
                    EndDate = a.EndDate,
                    Exp = a.Exp,
                    ProductId = a.ProductId,
                    Product = _mapper.Map<Product, ProductResponse>(a.Product),
                    StartDate = a.StartDate,
                    Status = a.Status,
                    CampaignDetails = new List<CampaignDetailResponse>
                     (a.CampaignDetails.Select(a => new CampaignDetailResponse
                     {
                         Id = a.Id,
                         MaxQuantity = a.MaxQuantity,
                         MinQuantity = a.MinQuantity,
                         UnitPrice = a.UnitPrice

                     }))
                }).SingleOrDefault();

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found campaign with id", response.Id.ToString());
                }

                return _mapper.Map<CampaignResponse>(response);

            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Category By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public Task<PagedResults<CampaignResponse>> GetCampaigns(CampaignRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<CampaignResponse>(request);
                var categorys = _unitOfWork.Repository<Campaign>().GetAll().Select(a=> new CampaignResponse
                {
                    Id = a.Id,
                    Discount = a.Discount,
                    EndDate = a.EndDate,
                    Exp=a.Exp,
                    ProductId=a.ProductId,
                    Product=_mapper.Map<Product,ProductResponse>(a.Product),
                    StartDate = a.StartDate,
                    Status = a.Status,
                    CampaignDetails = new List<CampaignDetailResponse>
                    (a.CampaignDetails.Select(a => new CampaignDetailResponse
                    {
                       Id=a.Id,
                       MaxQuantity=a.MaxQuantity,
                       MinQuantity=a.MinQuantity,
                       UnitPrice = a.UnitPrice

                    }))
                }) .ProjectTo<CampaignResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<CampaignResponse>.Sorting(paging.SortType, categorys, paging.ColName);
                var result = PageHelper<CampaignResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get campaign list error!!!!!", ex.Message);
            }
        }

        public Task<CampaignResponse> InsertCampaign(CampaignRequest category)
        {
            throw new NotImplementedException();
        }

        public Task<CampaignResponse> UpdateCampaign(int id, CampaignRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
