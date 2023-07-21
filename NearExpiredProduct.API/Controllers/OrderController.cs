using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Service;
using System.Data;

namespace NearExpiredProduct.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IReportService reportService;
        public OrderController(IOrderService orderService, IReportService reportService)
        {
            _orderService = orderService;
            this.reportService = reportService;
        }
        /// <summary>
        /// Get list of orders
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="ordRequest"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<OrderResponse>>> GetOrders([FromQuery] PagingRequest pagingRequest, [FromQuery] OrderRequest ordRequest)
        {
            var rs = await _orderService.GetOrders(ordRequest, pagingRequest);
            return Ok(rs);
        }
        /// <summary>
        /// Get order by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int id)
        {
            var rs = await _orderService.GetOrderById(id);
            return Ok(rs);
        }
        /// <summary>
        /// Get list of orders  by store Id
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpGet("storeId")]
        public async Task<ActionResult<List<OrderResponse>>> GetOrdersByStoreId([FromQuery] PagingRequest pagingRequest, int storeId)
        {
            var rs = await _orderService.GetOrderByStoreId(storeId, pagingRequest);
            return Ok(rs);
        }
        /// <summary>
        /// Get and update the status of the completed order
        /// </summary>
        /// <param name="ordId"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpGet("{ordId:int}/finished-orders")]
        public async Task<ActionResult<OrderResponse>> GetToUpdateOrderStatus(int ordId)
        {
            var rs = await _orderService.GetToUpdateOrderStatus(ordId);
            return Ok(rs);
        }
        /// <summary>
        /// Update information of order
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "store")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<OrderResponse>> UpdateOrder([FromBody] UpdateOrderRequest request, int id)
        {
            var rs = await _orderService.UpdateOrder(id, request);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [Authorize(Roles = "customer")]
        [HttpPost()]
        public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest order)
        {
            var rs = await _orderService.InsertOrder(order);
            return Ok(rs);
        }
        /// <summary>
        /// Get reports on admin orders
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet("order-report")]
        public async Task<ActionResult<dynamic>> GetReportOrder()
        {
            var rs = await reportService.GetOrdersReport();
            return Ok(rs);
        }
    }
}
