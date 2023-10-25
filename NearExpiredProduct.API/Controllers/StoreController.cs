using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IReportService reportService;
        public StoreController(IStoreService storeService, IReportService reportService)
        {
            _storeService = storeService;
            this.reportService = reportService;
        }
        /// <summary>
        /// Get All Stores
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="storeRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<StoreResponse>>> GetStores([FromQuery] PagingRequest pagingRequest, [FromQuery] StoreRequest storeRequest)
        {
            var rs = await _storeService.GetStores(storeRequest, pagingRequest);
            return Ok(rs);
        }
        /// <summary>
        /// Get the stores near the customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        [HttpGet("{id:int}/nearest-stores")]
        public async Task<ActionResult<List<StoreResponse>>> GetStoresSuitable(int id,[FromQuery] PagingRequest pagingRequest)
        {
            var rs = await _storeService.GetStoresSuitable(id,pagingRequest.PageSize,pagingRequest.Page);
            return Ok(rs);
        }
        /// <summary>
        /// Update information of store
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<StoreResponse>> UpdateStore([FromBody] UpdateStoreRequest storeRequest, int id)
        {
            var rs = await _storeService.UpdateStore(id, storeRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("authentication")]
        public async Task<ActionResult<StoreResponse>> Login([FromBody] LoginRequest model)
        {
            var rs = await _storeService.Login(model);
            return Ok(rs);
        }
        /// <summary>
        /// Get store by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StoreResponse>> GetStore(int id)
        {
            var rs = await _storeService.GetStoreById(id);
            return Ok(rs);
        }
        /// <summary>
        /// Get report for the time period of the store
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpGet("report-in-range")]
        public async Task<ActionResult<StoreReportModel>> GetReports([FromQuery] DateFilterRequest request)
        {
            var rs = await reportService.GetStoreDayReportInRange(request);
            return rs;
        }
        /// <summary>
        /// Get report by day of the store
        /// </summary>
        /// <param name="dayReport"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpGet("report-by-day")]
        public async Task<ActionResult<List<StoreDayReportModel>>> GetReportInDay([FromQuery] DateTime dayReport, int? storeId)
        {
            var rs = await reportService.GetStoreDayReportByDay(dayReport,storeId);
            return Ok(rs);
        }
        /// <summary>
        /// Register store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<StoreResponse>> CreateStore(CreateStoreRequest store)
        {
            var rs = await _storeService.CreateStore(store);
            return Ok(rs);
        }
        /// <summary>
        /// Reset password when forgot password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost("forgotten-password")]
        public async Task<ActionResult<CustomerResponse>> ResetPassword([FromQuery] ResetPasswordRequest resetPassword)
        {
            var rs = await _storeService.UpdatePass(resetPassword);
            return Ok(rs);
        }
       
    }
}
