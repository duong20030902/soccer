using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly SoccerContext _context;

        public OrdersController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingMethod)
                .Include(o => o.Address)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new OrderDto
                {
                    OrderID = o.OrderId,
                    UserName = o.User.FullName,
                    OrderDate = o.OrderDate,
                    ShippingMethod = o.ShippingMethod.MethodName,
                    Address = $"{o.Address.StreetAddress}, {o.Address.CityProvince}",
                    PaymentMethod = o.PaymentMethod.MethodName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.StatusName,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        UnitCost = oi.UnitCost,
                        Total = oi.Quantity * oi.UnitPrice
                    }).ToList()
                })
            .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingMethod)
                .Include(o => o.Address)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Status) 
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id); 

            if (order == null)
                return NotFound();

            // Create and return OrderDto
            var orderDto = new OrderDto
            {
                OrderID = order.OrderId,
                UserName = order.User.FullName,
                OrderDate = order.OrderDate,
                ShippingMethod = order.ShippingMethod.MethodName,
                Address = $"{order.Address.StreetAddress}, {order.Address.CityProvince}",
                PaymentMethod = order.PaymentMethod.MethodName,
                TotalAmount = order.TotalAmount,
                Status = order.Status.StatusName,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    UnitCost = oi.UnitCost,
                    Total = oi.Quantity * oi.UnitPrice
                }).ToList()
            };

            return Ok(orderDto);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] int statusId)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.StatusId = statusId;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
