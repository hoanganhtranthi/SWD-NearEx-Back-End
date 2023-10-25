using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;
using System.Data;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _userService;
        private readonly IFileStorageService _fileStorageService;
        public CustomerController(ICustomerService userService,IFileStorageService fileStorageService)
        {
            _userService = userService;
            _fileStorageService = fileStorageService;
        }
        /// <summary>
        /// Get list of customers
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<List<CustomerResponse>>> GetCustomers([FromQuery] PagingRequest pagingRequest, [FromQuery] CustomerRequest userRequest)
        {
            var rs = await _userService.GetCustomers(userRequest, pagingRequest);
            return Ok(rs);
        }
        /// <summary>
        /// Get customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer(int id)
        {
            var rs = await _userService.GetCustomerById(id);
            return Ok(rs);
        }
        /// <summary>
        /// Update profile of customer
        /// </summary>
        /// <param name="userRequest"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "customer")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> UpdateCustomer([FromBody] UpdateCustomerRequest userRequest, int id)
        {
            var rs = await _userService.UpdateCustomer(id, userRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> DeleteCustomer(int id)
        {
            var rs = await _userService.DeleteCustomer(id);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Send OTP 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="phone"></param>
        /// <param name="googleId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("verification")]
        public async Task<ActionResult<string>> Verification([FromBody] TwilioRequest request,[FromQuery] string? phone, [FromQuery] string? googleId)
        {
            var rs = await _userService.Verification(request,phone,googleId);
            return Ok(rs);
        }
        /// <summary>
        /// Sign Up account of customer by phone
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost()]
        public async Task<ActionResult<CustomerResponse>>CreateCustomer(CreateCustomerRequest customer)
        {
            var rs = await _userService.CreateCustomer(customer);
            return Ok(rs);
        }
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authentication")]
        public async Task<ActionResult<CustomerResponse>> Login([FromBody] LoginRequest model)
        {
            var rs = await _userService.Login(model);
            return Ok(rs);
        }
        /// <summary>
        /// Sign Up account of customer by googleId
        /// </summary>
        /// <param name="googleId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("google-authentication")]
        public async Task<ActionResult<CustomerResponse>> LoginGoogle([FromQuery] string googleId)
        {
            var rs = await _userService.LoginByGoogle(googleId);
            return Ok(rs);
        }
        /// <summary>
        /// Reset password when forgot password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("forgotten-password")]
        public async Task<ActionResult<CustomerResponse>> ResetPassword([FromQuery] ResetPasswordRequest resetPassword)
        {
            var rs = await _userService.UpdatePass(resetPassword);
            return Ok(rs);
        }
        /// <summary>
        /// Get JWT of account
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpGet("get-jwt/{id}")]
        public async Task<ActionResult<string>> GetJwt(int id)
        {
            var rs = await _userService.GetJwt(id);
            return Ok(rs);
        }
    }
}
