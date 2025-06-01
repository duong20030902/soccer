using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly SoccerContext _context;

        public OrderStatusController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatusDto>>> GetOrderStatuses()
        {
            var statuses = await _context.OrderStatuses
                .Select(os => new OrderStatusDto
                {
                    StatusID = os.StatusId,
                    StatusName = os.StatusName
                })
                .ToListAsync();

            return Ok(statuses);
        }

        [HttpPost]
        public async Task<ActionResult<OrderStatusDto>> CreateOrderStatus([FromBody] string statusName)
        {
            var status = new OrderStatus { StatusName = statusName };
            _context.OrderStatuses.Add(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderStatuses), null,
                new OrderStatusDto { StatusID = status.StatusId, StatusName = status.StatusName });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string statusName)
        {
            var status = await _context.OrderStatuses.FindAsync(id);
            if (status == null) return NotFound();

            status.StatusName = statusName;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderStatus(int id)
        {
            var status = await _context.OrderStatuses.FindAsync(id);
            if (status == null) return NotFound();

            _context.OrderStatuses.Remove(status);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
