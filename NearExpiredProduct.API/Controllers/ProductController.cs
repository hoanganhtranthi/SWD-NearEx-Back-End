using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponse>>> GetProducts([FromQuery] PagingRequest pagingRequest, [FromQuery] ProductRequest productRequest)
        {
            var rs = await _productService.GetProducts(productRequest, pagingRequest);
            return Ok(rs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            var rs = await _productService.GetProductById(id);
            return Ok(rs);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductResponse>> UpdateProduct([FromBody] ProductRequest productRequest, int id)
        {
            var rs = await _productService.UpdateProduct(id, productRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductResponse>> DeleteProduct(int id)
        {
            var rs = await _productService.DeleteProduct(id);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        [HttpPost()]
        public async Task<ActionResult<CustomerResponse>> CreateProduct([FromBody] ProductRequest model)
        {
            var rs = await _productService.CreateProduct(model);
            return Ok(rs);
        }

    }
}
