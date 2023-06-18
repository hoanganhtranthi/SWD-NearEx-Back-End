using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        Task<PagedResults<CampaignResponse>> GetCampaignByDate(CampaignRequest request, PagingRequest paging);
        Task<PagedResults<CampaignResponse>> GetCampaignByProductId(CampaignRequest request, PagingRequest paging);
        Task<CampaignResponse> UpdateCampaign(int id, CampaignRequest request);
        Task<CampaignResponse> InsertCampaign(CampaignRequest campaign);
    }
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = configuration;
        }
        public async Task<CampaignResponse> DeleteCampaign(int id)
        {
            var campaign = await _unitOfWork.Repository<Campaign>().GetAsync(c => c.Id == id);
            try
            {
                if (id <= 0)
                {
                    throw new Exception();
                }
                _unitOfWork.Repository<Campaign>().Delete(campaign);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Campaign, CampaignResponse>(campaign);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Campaign Error!!!", ex.InnerException?.Message);
            }
        }

        public Task<PagedResults<CampaignResponse>> GetCampaignByDate(CampaignRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<CampaignResponse>(request);
                var campaigns = _unitOfWork.Repository<Campaign>().GetAll().Where(c => c.StartDate == request.StartDate)
                                           .ProjectTo<CampaignResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<CampaignResponse>.Sorting(paging.SortType, campaigns, paging.ColName);
                var result = PageHelper<CampaignResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get campaign list by date error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<PagedResults<CampaignResponse>> GetCampaignByProductId(CampaignRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<CampaignResponse>(request);
                var campaigns = _unitOfWork.Repository<Campaign>().GetAll().Where(c => c.ProductId == request.ProductId)
                                           .ProjectTo<CampaignResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<CampaignResponse>.Sorting(paging.SortType, campaigns, paging.ColName);
                var result = PageHelper<CampaignResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get campaign list by product error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<PagedResults<CampaignResponse>> GetCampaigns(CampaignRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<CampaignResponse>(request);
                var campaigns = _unitOfWork.Repository<Campaign>().GetAll()
                                           .ProjectTo<CampaignResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<CampaignResponse>.Sorting(paging.SortType, campaigns, paging.ColName);
                var result = PageHelper<CampaignResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get campaign list error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<CampaignResponse> InsertCampaign(CampaignRequest campaign)
        {
            try
            {
                if (campaign == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Campaign Invalid!!!", "");
                }
                var cam = await _unitOfWork.Repository<Campaign>().GetAsync(c => c.Id == campaign.Id);
                _unitOfWork.Repository<Campaign>().CreateAsync(cam);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Campaign, CampaignResponse>(cam);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Campaign Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<CampaignResponse> UpdateCampaign(int id, CampaignRequest request)
        {
            try
            {
                Campaign campaign = null;
                campaign = _unitOfWork.Repository<Campaign>()
                    .Find(c => c.Id == id);

                if (campaign == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found campaign with id", id.ToString());
                }

                _mapper.Map<CampaignRequest, Campaign>(request, campaign);

                await _unitOfWork.Repository<Campaign>().Update(campaign, id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Campaign, CampaignResponse>(campaign);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update campaign error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
