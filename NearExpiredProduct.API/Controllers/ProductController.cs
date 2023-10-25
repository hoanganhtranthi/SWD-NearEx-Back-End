using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;
using static NearExpiredProduct.Service.Helpers.Enum;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        /// <summary>
        /// Get list of products
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="productRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductResponse>>> GetProducts([FromQuery] PagingRequest pagingRequest, [FromQuery] ProductRequest productRequest)
        {
            var rs = await _productService.GetProducts(productRequest, pagingRequest);
            return Ok(rs);
        }
        /// <summary>
        /// Load product name
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="productRequest"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpGet("product-names")]
        public async Task<ActionResult<List<dynamic>>> GetProductNames([FromQuery] int storeId)
        {
            var rs = await _productService.GetProductNames(storeId);
            return Ok(rs);
        }
        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            var rs = await _productService.GetProductById(id);
            return Ok(rs);
        }
        /// <summary>
        /// Update information of product
        /// </summary>
        /// <param name="productRequest"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductResponse>> UpdateProduct([FromBody] UpdateProductRequest productRequest, int id)
        {
            var rs = await _productService.UpdateProduct(id, productRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Update status of product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpPut("{id:int}/update-status")]
        public async Task<ActionResult<ProductResponse>> UpdateProductStatus(int id,ProductStatusEnum status)
        {
            var rs = await _productService.UpdateStatusProduct(id,status);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Create product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpPost()]
        public async Task<ActionResult<CustomerResponse>> CreateProduct([FromBody] CreateProductRequest model)
        {
            var rs = await _productService.CreateProduct(model);
            return Ok(rs);
        }

    }
}
