using AutoMapper;
using AutoMapper.QueryableExtensions;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
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
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Twilio.Http;

namespace NearExpiredProduct.Service.Service
{
    public interface IStoreService
    {
        Task<PagedResults<StoreResponse>> GetStores(StoreRequest request, PagingRequest paging);
        Task<StoreResponse> UpdateStore(int storeId, UpdateStoreRequest request);
        Task<StoreResponse> GetStoreById(int id);
        Task<StoreResponse> Login(LoginRequest request);
        Task<StoreResponse> CreateStore(CreateStoreRequest request);
        Task<StoreResponse> UpdatePass(ResetPasswordRequest request);
        Task<PagedResults<StoreResponse>> GetStoresSuitable(int cusId, int pageSize, int page);
        
    }
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public StoreService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }
        public async Task<PagedResults<StoreResponse>> GetStoresSuitable(int cusId, int pageSize, int page)
        {
            try 
            {
                var customer = _unitOfWork.Repository<Customer>().Find(a => a.Id == cusId);
                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with id {cusId}", "");
                }
                var listStore = _mapper.Map<List<StoreResponse>>(_unitOfWork.Repository<Store>().GetAll().ToList());
                var list=listStore.OrderBy(x => GeoJsonHelper.ParseStringToPoint(x.CoordinateString).Coordinates.ToList().Min(c => c.Distance(GeoJsonHelper.ParseStringToPoint(customer.CoordinateString).Coordinate)) * 40075.017 / 360).ToList();
                var rs= PageHelper<StoreResponse>.Paging(list, page, pageSize);
                return rs;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex.InnerException?.Message);
            }

        }
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
                    new Claim("FcmToken" , store.Fcmtoken ?? ""),
                    new Claim("ImageUrl", store.Logo ?? ""),
                    new Claim(ClaimTypes.MobilePhone , store.Phone),
                }),
                Expires = DateTime.UtcNow.AddYears(1),
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
                   .FirstOrDefault(s => s.Phone.Equals(request.Phone.Trim()));

                if (store == null) throw new CrudException(HttpStatusCode.BadRequest, "Store Not Found", "");

                if (!VerifyPasswordHash(request.Password.Trim(), store.PasswordHash, store.PasswordSalt))
                    throw new CrudException(HttpStatusCode.BadRequest, "Password is incorrect", "");
                var s = _mapper.Map<Store, StoreResponse>(store);
                s.Token = GenerateJwtToken(store);
                return Task.FromResult(_mapper.Map<StoreResponse>(s));
            }
            catch (CrudException ex)
            {
                throw ex;
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
        }

        public async Task<StoreResponse> UpdateStore(int storeId, UpdateStoreRequest request)
        {
            try
            {
                Store store = _unitOfWork.Repository<Store>().Find(s => s.Id == storeId);
                var stores = _unitOfWork.Repository<Store>().GetAll();

                if (stores != null)
                {
                    var s = stores.Where(s => s.StoreName.Equals(request.StoreName) && s.Address.Equals(request.Address)).SingleOrDefault(); 
                    if (s != null)
                    {
                        if (s.Id != storeId)
                        {
                            throw new CrudException(HttpStatusCode.BadRequest, "Store has already !!!", "");
                        }
                    }
                    if (request.OldPassword != null && request.NewPassword != null)
                    {
                        if (!VerifyPasswordHash(request.OldPassword.Trim(), store.PasswordHash, store.PasswordSalt))
                            throw new CrudException(HttpStatusCode.BadRequest, "Old Password is not match", "");
                        CreatPasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                        store.PasswordHash = passwordHash;
                        store.PasswordSalt = passwordSalt;
                    }
                }
               
                if (store == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found store with id {storeId.ToString()}", "");
                }
               
                _mapper.Map<UpdateStoreRequest, Store>(request, store);
                await _unitOfWork.Repository<Store>().Update(store, store.Id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Store, StoreResponse>(store);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update store error!!!!!", ex.Message);
            }
        }

        public async Task<StoreResponse> DeleteStore(int id)
        {
            var store = await _unitOfWork.Repository<Store>().GetAsync(s => s.Id == id);
            try
            {
                if (store == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found store with id{id.ToString()}", "");
                }
                _unitOfWork.Repository<Store>().Delete(store);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Store, StoreResponse>(store);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Store Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<StoreResponse> GetStoreById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Store Invalid", "");
                }
                var response = await _unitOfWork.Repository<Store>().GetAsync(c => c.Id == id);

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found store with id{id.ToString()}", "");
                }

                return _mapper.Map<StoreResponse>(response);

            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<StoreResponse> CreateStore(CreateStoreRequest request)
        {
            try
            {
                var store = _mapper.Map<CreateStoreRequest, Store>(request);
                var s = _unitOfWork.Repository<Store>().Find(s => s.StoreName == request.StoreName && s.Address.Equals(request.Address));
                if (s != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Store has already !!!", "");
                }
                CreatPasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                store.PasswordHash = passwordHash;
                store.PasswordSalt = passwordSalt;
                await _unitOfWork.Repository<Store>().CreateAsync(store);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Store, StoreResponse>(store);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create Product Error!!!", ex?.Message);
            }
        }
        private void CreatPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        public async Task<StoreResponse> UpdatePass(ResetPasswordRequest request)
        {
            try
            {
                Store store = null;
                store = _unitOfWork.Repository<Store>()
                    .Find(c => c.Phone.Equals(request.Phone));
                if (store == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found store with phone{request.Phone}", "");
                }
                CreatPasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                store.PasswordHash = passwordHash;
                store.PasswordSalt = passwordSalt;
                await _unitOfWork.Repository<Store>().Update(store, store.Id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Store, StoreResponse>(store);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Password customer error!!!!!", ex.Message);
            }
        }
    }
}
