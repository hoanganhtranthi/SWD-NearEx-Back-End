using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;
using System.Data;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/campaigns")]
    [ApiController]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;
        public CampaignController(ICampaignService campaginService)
        {
            _campaignService = campaginService;
        }
        /// <summary>
        /// Get list of campaigns
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="campaignRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<CampaignResponse>>> GetCampaigns([FromQuery] PagingRequest pagingRequest, [FromQuery] CampaignRequest campaignRequest)
        {
            var rs = await _campaignService.GetCampaigns(campaignRequest, pagingRequest);
            return Ok(rs);
        }
       /// <summary>
       /// Get campaign by Id
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CampaignResponse>> GetCampaign(int id)
        {
            var rs = await _campaignService.GetCampaignById(id);
            return Ok(rs);
        }
        /// <summary>
        /// Get list of campaigns by store Id
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        //[Authorize(Roles = "store")]
        [HttpGet("store")]
        public async Task<ActionResult<PagedResults<CampaignResponse>>> GetCampaignByStore([FromQuery]int storeId, [FromQuery] PagingRequest paging)
        {
            var rs = await _campaignService.GetCampaignByStore(storeId, paging);
            return Ok(rs);
        }
        /// <summary>
        /// Create a customer's favorite campaign
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "customer")]
        [HttpPost("wishlist")]
        public async Task<ActionResult<CustomerResponse>> CreateWishList([FromBody]WishListRequest request)
        {
            var rs = await _campaignService.AddWishList(request);
            return Ok(rs);
        }
        /// <summary>
        /// Get a list of customer's favorite campaigns
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [Authorize(Roles = "customer")]
        [HttpGet("{id:int}/wishlists")]
        public async Task<ActionResult<CustomerResponse>> GetWishList(int id, [FromQuery] PagingRequest paging)
        {
            var rs = await _campaignService.GetWishList(id,paging);
            return Ok(rs);
        }
        /// <summary>
        /// Delete a  customer's favorite campaign
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "customer")]
        [HttpDelete("wishlist")]
        public async Task<ActionResult<CustomerResponse>> DeleteWishList([FromQuery] WishListRequest request)
        {
            var rs = await _campaignService.DeleteWishList(request);
            return Ok(rs);
        }
        /// <summary>
        /// Get list of campaigns by category Id
        /// </summary>
        /// <param name="cateId"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("cate")]
        public async Task<ActionResult<PagedResults<CampaignResponse>>> GetCampaignByCategory(int cateId, [FromQuery] PagingRequest paging)
        {
            var rs = await _campaignService.GetCampaignByCategory(cateId, paging);
            return Ok(rs);
        }
        /// <summary>
        /// Count the number of campagins by category of each store
        /// </summary>
        /// <param name="cateId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("stores")]
        public async Task<ActionResult<List<CampaignAmoutByStoreRequest>>> GetCountCampaignByStore([FromQuery]int cateId)
        {
            var rs = await _campaignService.GetCountCampaignByStore(cateId);
            return Ok(rs);
        }
        /// <summary>
        /// Create campaign
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpPost()]
        public async Task<ActionResult<CampaignResponse>> CreateCampagin([FromBody] CreateCampaginRequest model)
        {
            var rs = await _campaignService.InsertCampaign(model);
            return Ok(rs);
        }
        /// <summary>
        /// Get and update the status of campaigns as out of stock
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize(Roles = "store")]
        [HttpGet("{id:int}/out-of-stock-campaigns")]
        public async Task<ActionResult<CampaignResponse>> GetToUpdateCampaignStatus(int id)
        {
                var rs = await _campaignService.GetToUpdateCampaignStatus(id);
                return Ok(rs);
        }
        /// <summary>
        /// Update information of campaign or add campaign details to a campaign
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CampaignResponse>> UpdateCampaign([FromBody] UpdateCampaignRequest request, int id)
        {
            var rs = await _campaignService.UpdateCampaign(id, request);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Get list of campaigns with the most purchases
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("best-seller-campaign")]
        public async Task<ActionResult<PagedResults<CampaignResponse>>> GetCampaignBestSeller([FromQuery] PagingRequest paging)
        {
            var rs = await _campaignService.GetBestSellerCampaign(paging);
            return Ok(rs);
        }
        /// <summary>
        /// Get the list of most favorite campaigns
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("favourite-campaign")]
        public async Task<ActionResult<PagedResults<CampaignResponse>>> GetFavouriteCampaign([FromQuery] PagingRequest paging)
        {
            var rs = await _campaignService.GetFavoriteCampaign(paging);
            return Ok(rs);
        }
    }
}
