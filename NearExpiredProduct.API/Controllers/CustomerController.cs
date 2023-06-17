using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;
using System.Data;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _userService;
        public CustomerController(ICustomerService userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<List<CustomerResponse>>> GetCustomers([FromQuery] PagingRequest pagingRequest, [FromQuery] CustomerRequest userRequest)
        {
            var rs = await _userService.GetCustomers(userRequest, pagingRequest);
            return Ok(rs);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer(int id)
        {
            var rs = await _userService.GetCustomerById(id);
            return Ok(rs);
        }

        [Authorize(Roles = "customer")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> UpdateCustomer([FromBody] CustomerRequest userRequest, int id)
        {
            var rs = await _userService.UpdateCustomer(id, userRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        //[Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> DeleteCustomer(int id)
        {
            var rs = await _userService.DeleteCustomer(id);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        [Authorize(Roles = "customer")]
        [HttpPost("verification")]
        public async Task<ActionResult<string>> Verification([FromQuery] string phone, [FromQuery] string? code)
        {
            var rs = await _userService.Verification(phone,code);
            return Ok(rs);
        }
        [HttpPost("login")]
        public async Task<ActionResult<CustomerResponse>> Login([FromBody] LoginRequest model)
        {
            var rs = await _userService.Login(model);
            return Ok(rs);
        }
        [Authorize(Roles = "customer")]
        [HttpPost("resetPassword")]
        public async Task<ActionResult<CustomerResponse>> ResetPassword([FromQuery] ResetPasswordRequest resetPassword, bool resetPass, [FromQuery] string email)
        {
            var rs = await _userService.ResetPassword(resetPass, resetPassword, email);
            return Ok(rs);
        }
    }
}
