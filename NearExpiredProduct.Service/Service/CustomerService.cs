using AutoMapper;
using AutoMapper.QueryableExtensions;
using NearExpiredProduct.Service.Helpers;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Data.UnitOfWork;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Exceptions;
using NearExpiredProduct.Service.Helpers;
using NearExpiredProduct.Service.Utilities;
using NTQ.Sdk.Core.CustomModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.Service
{
    public interface ICustomerService
    {
        Task<PagedResults<CustomerResponse>> GetCustomers(CustomerRequest request, PagingRequest paging);
        Task<CustomerResponse> LinkToGoogleAccount(ExternalAuthRequest data);
        Task<CustomerResponse> DeleteCustomer(int id);
        Task<CustomerResponse> ResetPassword(bool forgotPass, ResetPasswordRequest resetPassword, string email);
        Task<CustomerResponse> Login(LoginRequest request);
        Task<CustomerResponse> Registeration(CustomerRequest request);
        Task<CustomerResponse> GetCustomerById(int id);
        Task<CustomerResponse> UpdateCustomer(int customerId, CustomerRequest request);
    }
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = configuration;
        }
        public async Task<CustomerResponse> Registeration(CustomerRequest request)
        {
            try
            {
                var isUnique = IsUniqueUser(request.Email);
                if (!isUnique)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "User has already register", "");
                }
                if (request == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "User information is invalid", "");
                }
                var customer = _mapper.Map<CustomerRequest, Customer>(request);
                await _unitOfWork.Repository<Customer>().CreateAsync(customer);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Customer, CustomerResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create User Error!!!", ex?.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool IsUniqueUser(string Email)
        {
            var user = _unitOfWork.Repository<Customer>().Find(u => u.Email == Email);
            if (user == null)
            {
                return true;
            }
            return false;
        }      
        public async Task<CustomerResponse> GetCustomerById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id User Invalid", "");
                }
                var response = await _unitOfWork.Repository<Customer>().GetAsync(u => u.Id == id);

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found customer with id", response.Id.ToString());
                }

                return _mapper.Map<CustomerResponse>(response);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get User By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public Task<PagedResults<CustomerResponse>> GetCustomers(CustomerRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<CustomerResponse>(request);
                var customers = _unitOfWork.Repository<Customer>().GetAll()
                                           .ProjectTo<CustomerResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<CustomerResponse>.Sorting(paging.SortType, customers, paging.ColName);
                var result = PageHelper<CustomerResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get customer list error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<CustomerResponse> LinkToGoogleAccount(ExternalAuthRequest data)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
            // Change this to your google client ID
            settings.Audience = new List<string>() { "458807280767-qb3n290oka2phviu8rf3c7opdsg00nn4.apps.googleusercontent.com" };

            GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;

            CustomerResponse newCustomer = new CustomerResponse()
            {
                UserName = payload.Name,
                Email = payload.Email
            };
            return newCustomer;
        }

        public async Task<CustomerResponse> UpdateCustomer(int customerId, CustomerRequest request)
        {
            try
            {
                Customer customer = null;
                customer = _unitOfWork.Repository<Customer>()
                    .Find(c => c.Id == customerId);

                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found customer with id", customerId.ToString());
                }

                _mapper.Map<CustomerRequest, Customer>(request, customer);

                await _unitOfWork.Repository<Customer>().Update(customer, customer.Id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Customer, CustomerResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update customer error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private string GenerateJwtToken(Customer customer)
        {
            string role;
            if (customer.Email.Equals(_config["AdminAccount:Email"]) && customer.CustomerPassword.Equals(_config["AdminAccount:Password"]))
                role = "admin";
            else role = "customer";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["ApiSetting:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name , customer.UserName),
                new Claim(ClaimTypes.Email , customer.Email),
                new Claim("FcmToken" , customer.Fcmtoken ?? ""),
                new Claim("ImageUrl", customer.Avatar ?? ""),
                new Claim(ClaimTypes.MobilePhone , customer.Phone),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<CustomerResponse> Login(LoginRequest request)
        {
            try
            {
                var user = _unitOfWork.Repository<Customer>().GetAll()
                   .FirstOrDefault(u => u.Email.Equals(request.Email.Trim()));

                if (user == null) throw new CrudException(HttpStatusCode.BadRequest, "User Not Found", "");
                if (!user.CustomerPassword.Equals(request.Password.Trim()))
                    throw new CrudException(HttpStatusCode.BadRequest, "Password is incorrect", "");
                var cus = _mapper.Map<Customer, CustomerResponse>(user);
                cus.Token = GenerateJwtToken(user);
                return cus;
                return _mapper.Map<CustomerResponse>(user);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex.InnerException?.Message);
            }


        }
        public async Task<CustomerResponse> DeleteCustomer(int id)
        {
            var user = await _unitOfWork.Repository<Customer>().GetAsync(u => u.Id == id);
            try
            {
                if (id <= 0)
                {
                    throw new Exception();
                }
                _unitOfWork.Repository<Customer>().Delete(user);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Customer, CustomerResponse>(user);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete User Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<CustomerResponse> ResetPassword(bool forgotPass, ResetPasswordRequest resetPassword, string email)
        {
            try
            {
                var user = _unitOfWork.Repository<Customer>().GetAll()
                   .FirstOrDefault(u => u.Email.Equals(email.Trim()));

                if (user == null) throw new CrudException(HttpStatusCode.BadRequest, "User Not Found", "");
                if (forgotPass)
                {

                    user.PasswordResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)); ;
                    user.ResetTokenExpires = DateTime.Now.AddDays(1);

                    await _unitOfWork.Repository<Customer>().Update(user, user.Id);
                    await _unitOfWork.CommitAsync();

                }

                else if (resetPassword.ConfirmPassword != null)
                {
                    if (user.PasswordResetToken != resetPassword.PasswordResetToken.Trim() || user.ResetTokenExpires < DateTime.Now)
                        throw new CrudException(HttpStatusCode.BadRequest, "Invalid Information", "");
                    if (!resetPassword.Password.Equals(resetPassword.ConfirmPassword))
                        throw new CrudException(HttpStatusCode.BadRequest, "Password and confirm password are not match!!!!!", "");

                    user.CustomerPassword = resetPassword.Password;

                    await _unitOfWork.Repository<Customer>().Update(user, user.Id);
                    await _unitOfWork.CommitAsync();
                }
                return _mapper.Map<CustomerResponse>(user);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex.InnerException?.Message);
            }
        }
    }
}
