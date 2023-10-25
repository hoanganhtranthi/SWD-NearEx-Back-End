using AutoMapper;
using AutoMapper.QueryableExtensions;
using NearExpiredProduct.Service.Helpers;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Data.UnitOfWork;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Exceptions;
using NearExpiredProduct.Service.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using System.Security.Principal;
using Twilio.Http;
using Firebase.Auth;
using static Google.Rpc.Context.AttributeContext.Types;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;

namespace NearExpiredProduct.Service.Service
{
    public interface ICustomerService
    {
        Task<PagedResults<CustomerResponse>> GetCustomers(CustomerRequest request, PagingRequest paging);
        Task<CustomerResponse> DeleteCustomer(int id);
        Task<CustomerResponse> GetCustomerByEmail(string email);
        Task<string> Verification(TwilioRequest request, string phone, string googleId);
        Task<CustomerResponse> Login(LoginRequest request);
        Task<CustomerResponse> LoginByGoogle(string googleId);
        Task<CustomerResponse> CreateCustomer(CreateCustomerRequest request);
        Task<CustomerResponse> GetCustomerById(int id);
        Task<CustomerResponse> GetMe(int accountId);
        Task<string> GetJwt(int accountId);
        Task<CustomerResponse> UpdatePass(ResetPasswordRequest request);
        Task<CustomerResponse> UpdateCustomer(int customerId, UpdateCustomerRequest request);
    }
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        // private string accountSID, token,serviceID;
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = configuration;
            /*accountSID = _config["Twilio:AccountSID"];
            token = _config["Twilio:AuthToken"];
            serviceID = _config["Twilio:PathServiceSid"];*/
        }
        public static bool CheckVNPhone(string phoneNumber)
        {
            string strRegex = @"(^(0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$)";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(phoneNumber))
            {
                return true;
            }
            else
                return false;
        }

        public async Task<string> Verification(TwilioRequest request, string phone, string googleId)
        {
            try
            {
                if (phone != null || googleId != null)
                {
                    if (!IsUniqueUser(phone, googleId)) return "true";
                    else return "false";
                }
                else
                {
                    #region checkPhone
                    var checkPhone = CheckVNPhone(request.Phone);
                    if (checkPhone)
                    {
                        if (!request.Phone.StartsWith("+84"))
                        {
                            request.Phone = request.Phone.TrimStart(new char[] { '0' });
                            request.Phone = "+84" + request.Phone;
                        }
                    }
                    else
                    {
                        throw new CrudException(HttpStatusCode.BadRequest, "Wrong Phone", request.Phone.ToString());
                    }
                    #endregion
                    TwilioClient.Init(request.AccountSID, request.AuthToken);

                    if (request.Token != null)
                    {
                        var verificationCheck = VerificationCheckResource.Create(
                        to: request.Phone,
                        code: request.Token,
                        pathServiceSid: request.PathServiceSid
                    );

                        return verificationCheck.Status;
                    }
                    else
                    {
                        var verification = VerificationResource.Create(
                            channel: "sms",
                            to: request.Phone,
                            pathServiceSid: request.PathServiceSid
                    );
                        return verification.Status;
                    }
                }
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Verification Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<CustomerResponse> GetCustomerByEmail(string email)
        {
            try
            {
                Customer customer = null;
                customer = _unitOfWork.Repository<Customer>().GetAll()
                    .Where(x => x.Email.Contains(email)).FirstOrDefault();

                return _mapper.Map<Customer, CustomerResponse>(customer);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool IsUniqueUser(string Phone, string googleId)
        {
            if (googleId != null)
            {
                var user = _unitOfWork.Repository<Customer>().Find(u => u.GoogleId == googleId);
                if (user == null)
                {
                    return true;
                }
                return false;
            }
            else
            {
                var user = _unitOfWork.Repository<Customer>().Find(u => u.Phone == Phone);
                if (user == null)
                {
                    return true;
                }
                return false;
            }
        }
        public async Task<CustomerResponse> GetCustomerById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Customer Invalid", "");
                }
                var response = await _unitOfWork.Repository<Customer>().GetAsync(u => u.Id == id);

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with id {id.ToString()}", "");
                }

                return _mapper.Map<CustomerResponse>(response);
            }
            catch (CrudException ex)
            {
                throw ex;
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
        }
        public async Task<CustomerResponse> CreateCustomer(CreateCustomerRequest request)
        {
            try
            {
                var customer = _mapper.Map<CreateCustomerRequest, Customer>(request);
                var s = _unitOfWork.Repository<Customer>().Find(s => s.Phone == request.Phone);
                if (s != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Phone has already !!!", "");
                }
                var cus = _unitOfWork.Repository<Customer>().Find(s => s.Email == request.Email);
                if (cus != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Email has already !!!", "");
                }
                CreatPasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                customer.PasswordHash = passwordHash;
                customer.PasswordSalt = passwordSalt;
                await _unitOfWork.Repository<Customer>().CreateAsync(customer);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Customer, CustomerResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create Customer Error!!!", ex?.Message);
            }
        }


        public async Task<CustomerResponse> UpdateCustomer(int customerId, UpdateCustomerRequest request)
        {
            try
            {
                Customer customer = null;
                customer = _unitOfWork.Repository<Customer>()
                    .Find(c => c.Id == customerId);

                var cus = _unitOfWork.Repository<Customer>()
                    .GetAll().Where(c => c.Phone.Equals(request.Phone)).SingleOrDefault();

                var rs = _unitOfWork.Repository<Customer>()
                    .GetAll().Where(c => c.Email.Equals(request.Email)).SingleOrDefault();
                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with id{customerId.ToString()}", "");
                }
                if (cus != null && cus.Id != customerId)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Phone has already !!!", "");
                }
                if (rs != null && rs.Id != customerId)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Email has already !!!", "");
                }
                _mapper.Map<UpdateCustomerRequest, Customer>(request, customer);
                if (request.OldPassword != null && request.NewPassword != null)
                {
                    if (!VerifyPasswordHash(request.OldPassword.Trim(), cus.PasswordHash, cus.PasswordSalt))
                        throw new CrudException(HttpStatusCode.BadRequest, "Old Password is not match", "");
                    CreatPasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    customer.PasswordHash = passwordHash;
                    customer.PasswordSalt = passwordSalt;
                }
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
                throw new CrudException(HttpStatusCode.BadRequest, "Update customer error!!!!!", ex.Message);
            }
        }
        private string GenerateJwtToken(Customer customer)
        {
            string role;
            string pass = _config["AdminAccount:Password"];
            if (customer.Phone.Equals(_config["AdminAccount:Phone"]) && VerifyPasswordHash(pass.Trim(), customer.PasswordHash, customer.PasswordSalt))
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
                new Claim("GoogleId", customer.GoogleId ?? ""),
                new Claim(ClaimTypes.MobilePhone , customer.Phone),
                }),
                Expires = DateTime.UtcNow.AddYears(1),
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
                   .FirstOrDefault(u => u.Phone.Equals(request.Phone.Trim()));

                if (user == null) throw new CrudException(HttpStatusCode.BadRequest, "User Not Found", "");
                if (!VerifyPasswordHash(request.Password.Trim(), user.PasswordHash, user.PasswordSalt))
                    throw new CrudException(HttpStatusCode.BadRequest, "Password is incorrect", "");
                var cus = _mapper.Map<Customer, CustomerResponse>(user);
                cus.Token = GenerateJwtToken(user);
                return cus;
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
        public async Task<CustomerResponse> DeleteCustomer(int id)
        {
            var user = await _unitOfWork.Repository<Customer>().GetAsync(u => u.Id == id);
            try
            {
                if (user == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with id {id.ToString()}", "");
                }
                _unitOfWork.Repository<Customer>().Delete(user);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Customer, CustomerResponse>(user);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete User Error!!!", ex.InnerException?.Message);
            }
        }
        public async Task<CustomerResponse> GetMe(int accountId)
        {
            try
            {
                if (accountId <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Customer Invalid", "");
                }
                var response = await _unitOfWork.Repository<Customer>().GetAsync(u => u.Id == accountId);

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with id {accountId.ToString()}", "");
                }

                return _mapper.Map<CustomerResponse>(response);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get User By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<CustomerResponse> UpdatePass(ResetPasswordRequest request)
        {
            try
            {
                Customer customer = null;
                customer = _unitOfWork.Repository<Customer>()
                    .Find(c => c.Phone.Equals(request.Phone));
                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with phone{request.Phone}","");
                }
                CreatPasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                customer.PasswordHash = passwordHash;
                customer.PasswordSalt = passwordSalt;
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
                throw new CrudException(HttpStatusCode.BadRequest, "Update Password customer error!!!!!", ex.Message);
            }
        }
        public async Task<string> GetJwt(int accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Customer>().GetById(accountId);
                if (account == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with id {accountId}", "");
                }
                return GenerateJwtToken(account);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Jwt error!!!!!", ex.Message);
            }
        }
        public async Task<CustomerResponse> LoginByGoogle(string googleId)
        {
            try
            {
                var user = _unitOfWork.Repository<Customer>().GetAll()
                   .FirstOrDefault(u => u.GoogleId.Equals(googleId.Trim()));

                if (user == null) throw new CrudException(HttpStatusCode.BadRequest, $"User Not Found with googleId {googleId}", "");
                var cus = _mapper.Map<Customer, CustomerResponse>(user);
                cus.Token = GenerateJwtToken(user);
                return cus;
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

       
    }
}
