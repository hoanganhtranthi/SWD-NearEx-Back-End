using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Data.UnitOfWork;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Exceptions;
using NearExpiredProduct.Service.Helpers;
using NearExpiredProduct.Service.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.Service
{
    public interface IStoreService
    {
        Task<PagedResults<StoreResponse>> GetStores(StoreRequest request, PagingRequest paging);
        Task<StoreResponse> UpdateStore(int storeId, StoreRequest request);
        Task<StoreResponse> DeleteStore(int id);
        Task<StoreResponse> Login(LoginRequest request);
    }
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        private string GenerateJwtToken(Store store)
        {
            string role = "store";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["ApiSetting:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, store.Id.ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Name, store.StoreName),
                    new Claim(ClaimTypes.Email, store.StoreAccount),
                    new Claim("FcmToken" , store.Fcmtoken ?? ""),
                    new Claim("ImageUrl", store.Logo ?? ""),
                    new Claim(ClaimTypes.MobilePhone , store.Phone),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<StoreResponse> Login(LoginRequest request)
        {
            try
            {
                var store = _unitOfWork.Repository<Store>().GetAll()
                   .FirstOrDefault(s => s.StoreAccount.Equals(request.Email.Trim()));

                if (store == null) throw new CrudException(HttpStatusCode.BadRequest, "Store Not Found", "");
                if (!store.Password.Equals(request.Password.Trim()))
                    throw new CrudException(HttpStatusCode.BadRequest, "Password is incorrect", "");
                var s = _mapper.Map<Store, StoreResponse>(store);
                s.Token = GenerateJwtToken(store);
                return Task.FromResult(_mapper.Map<StoreResponse>(s));
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex.InnerException?.Message);
            }
        }

        public Task<PagedResults<StoreResponse>> GetStores(StoreRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<StoreResponse>(request);
                var stores = _unitOfWork.Repository<Store>().GetAll()
                                           .ProjectTo<StoreResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<StoreResponse>.Sorting(paging.SortType, stores, paging.ColName);
                var result = PageHelper<StoreResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get store list error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<StoreResponse> UpdateStore(int storeId, StoreRequest request)
        {
            try
            {
                Store store = null;
                store = _unitOfWork.Repository<Store>().Find(s => s.Id == storeId);

                if (store == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found store with id", storeId.ToString());
                }

                _mapper.Map<StoreRequest, Store>(request, store);
                await _unitOfWork.Repository<Store>().Update(store, store.Id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Store, StoreResponse>(store);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update store error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<StoreResponse> DeleteStore(int id)
        {
            var store = await _unitOfWork.Repository<Store>().GetAsync(s => s.Id == id);
            try
            {
                if (id <= 0)
                {
                    throw new Exception();
                }
                _unitOfWork.Repository<Store>().Delete(store);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Store, StoreResponse>(store);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Store Error!!!", ex.InnerException?.Message);
            }
        }
    }
}
