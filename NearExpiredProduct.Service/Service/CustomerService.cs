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

namespace NearExpiredProduct.Service.Service
{
    public interface ICustomerService
    {
        Task<PagedResults<CustomerResponse>> GetCustomers(CustomerRequest request, PagingRequest paging);
        Task<CustomerResponse> LoginByGoogleAccount(ExternalAuthRequest data);
        Task<CustomerResponse> DeleteCustomer(int id);
        Task<CustomerResponse> GetCustomerByEmail(string email);
        Task<string> Verification(string phone,string code);
        Task<CustomerResponse> ResetPassword(bool forgotPass, ResetPasswordRequest resetPassword, string email);
        Task<CustomerResponse> Login(LoginRequest request);
        Task<CustomerResponse> GetCustomerById(int id);
        Task<CustomerResponse> UpdateCustomer(int customerId, CustomerRequest request);
    }
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private string accountSID, token,serviceID;
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = configuration;
            accountSID = _config["Twilio:AccountSID"];
            token = _config["Twilio:AuthToken"];
            serviceID = _config["Twilio:PathServiceSid"];
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
     
        public async Task<string> Verification(string phone,string code)
        {
            try
            {
                if (!IsUniqueUser(phone))
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "User has already register", "");
                }

                #region checkPhone
                var check = CheckVNPhone(phone);
                if (check)
                {
                    if (!phone.StartsWith("+84"))
                    {
                        phone = phone.TrimStart(new char[] { '0' });
                        phone = "+84" + phone;
                    }
                }
                else
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Wrong Phone", phone.ToString());
                }
                #endregion
                TwilioClient.Init(accountSID, token);

                if (code != null)
                {
                    var verificationCheck = VerificationCheckResource.Create(
                    to: phone,
                    code: code,
                    pathServiceSid: serviceID
                );

                    return verificationCheck.Status;
                }
                else
                {
                    var verification = VerificationResource.Create(
                        channel: "sms",
                        to: phone,
                        pathServiceSid: serviceID
                );
                    return verification.Status;
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
        public bool IsUniqueUser(string Phone)
        {
            var user = _unitOfWork.Repository<Customer>().Find(u => u.Phone==Phone);
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
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Customer Invalid", "");
                }
                var response = await _unitOfWork.Repository<Customer>().GetAsync(u => u.Id == id);

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found customer with id", response.Id.ToString());
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
        public async Task<CustomerResponse> CreateCustomer(CustomerRequest request)
        {
            try
            {
                var customer = _mapper.Map<CustomerRequest, Customer>(request);

                await _unitOfWork.Repository<Customer>().CreateAsync(customer);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Customer, CustomerResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create Product Error!!!", ex?.Message);
            }
        }
        public async Task<CustomerResponse> LoginByGoogleAccount(ExternalAuthRequest data)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
            // Change this to your google client ID
            settings.Audience = new List<string>() { "458807280767-qb3n290oka2phviu8rf3c7opdsg00nn4.apps.googleusercontent.com" };

            GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;
            var customer = await GetCustomerByEmail(payload.Email);
            if (customer == null)
            {
                CustomerRequest newCustomer = new CustomerRequest()
                {
                    UserName = payload.Name,
                    Email = payload.Email,
                    Avatar = payload.Picture
                };
                await CreateCustomer(newCustomer);
            }
            return customer;
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
            if (customer.Email.Equals(_config["AdminAccount:Email"]) && customer.Password.Equals(_config["AdminAccount:Password"]))
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
                #region checkPhone
                var check = CheckVNPhone(request.Phone);
                if (check)
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

                var user = _unitOfWork.Repository<Customer>().GetAll()
                   .FirstOrDefault(u => u.Phone.Equals(request.Phone.Trim()));

                if (user == null) throw new CrudException(HttpStatusCode.BadRequest, "User Not Found", "");
                if (!user.Password.Equals(request.Password.Trim()))
                    throw new CrudException(HttpStatusCode.BadRequest, "Password is incorrect", "");
                var cus = _mapper.Map<Customer, CustomerResponse>(user);
                cus.Token = GenerateJwtToken(user);
                return cus;
                return _mapper.Map<CustomerResponse>(user);
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
                if(user == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found customer with id", id.ToString());
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

                    user.Password = resetPassword.Password;

                    await _unitOfWork.Repository<Customer>().Update(user, user.Id);
                    await _unitOfWork.CommitAsync();
                }
                return _mapper.Map<CustomerResponse>(user);
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
    }
}
