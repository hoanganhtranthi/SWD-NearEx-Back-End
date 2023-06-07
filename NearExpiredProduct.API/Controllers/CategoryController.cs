using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryResponse>>> GetCategorys([FromQuery] PagingRequest pagingRequest, [FromQuery] CategoryRequest categoryRequest)
        {
            var rs = await _categoryService.GetCategorys(categoryRequest, pagingRequest);
            return Ok(rs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> GetCategory(int id)
        {
            var rs = await _categoryService.GetCategoryById(id);
            return Ok(rs);
        }

        [HttpGet("getCategoryByName")]
        public async Task<ActionResult<CategoryResponse>> GetCategoryByName(string name)
        {
            var rs = await _categoryService.GetCategoryByName(name);
            return Ok(rs);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> UpdateCategory([FromBody] CategoryRequest categoryRequest, int id)
        {
            var rs = await _categoryService.UpdateCategory(id, categoryRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> DeleteCategory(int id)
        {
            var rs = await _categoryService.DeleteCategory(id);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

    }
}
