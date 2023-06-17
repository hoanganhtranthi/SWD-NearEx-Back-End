using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/store")]
    [ApiController]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StoreResponse>>> GetStores([FromQuery] PagingRequest pagingRequest, [FromQuery] StoreRequest storeRequest)
        {
            var rs = await _storeService.GetStores(storeRequest, pagingRequest);
            return Ok(rs);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<StoreResponse>> UpdateStore([FromBody] StoreRequest storeRequest, int id)
        {
            var rs = await _storeService.UpdateStore(id, storeRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<StoreResponse>> DeleteStore(int id)
        {
            var rs = await _storeService.DeleteStore(id);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        [HttpPost("login")]
        public async Task<ActionResult<CustomerResponse>> Login([FromBody] LoginRequest model)
        {
            var rs = await _storeService.Login(model);
            return Ok(rs);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StoreResponse>> GetStore(int id)
        {
            var rs = await _storeService.GetStoreById(id);
            return Ok(rs);
        }
    }
}
