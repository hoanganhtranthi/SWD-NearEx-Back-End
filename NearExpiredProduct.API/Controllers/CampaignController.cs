using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/campaign")]
    [ApiController]
    public class CampaignController : Controller
    {
        private readonly CampaignService _campaignService;
        public CampaignController(CampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CampaignResponse>>> GetCampaigns([FromQuery] PagingRequest pagingRequest, [FromQuery] CampaignRequest campaignRequest)
        {
            var rs = await _campaignService.GetCampaigns(campaignRequest, pagingRequest);
            return Ok(rs);
        }

        [HttpGet]
        public async Task<ActionResult<List<CampaignResponse>>> GetCampaignByDate([FromQuery] PagingRequest pagingRequest, [FromQuery] CampaignRequest campaignRequest)
        {
            var rs = await _campaignService.GetCampaignByDate(campaignRequest, pagingRequest);
            return Ok(rs);
        }

        [HttpGet]
        public async Task<ActionResult<List<CampaignResponse>>> GetCampaignByProductId([FromQuery] PagingRequest pagingRequest, [FromQuery] CampaignRequest campaignRequest)
        {
            var rs = await _campaignService.GetCampaignByProductId(campaignRequest, pagingRequest);
            return Ok(rs);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CampaignResponse>> UpdateCampaign([FromBody] CampaignRequest campaignRequest, int id)
        {
            var rs = await _campaignService.UpdateCampaign(id, campaignRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CampaignResponse>> DeleteCampaign(int id)
        {
            var rs = await _campaignService.DeleteCampaign(id);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
    }
}
