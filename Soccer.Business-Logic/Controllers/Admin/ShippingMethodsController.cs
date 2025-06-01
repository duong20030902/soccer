using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ShippingMethodsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public ShippingMethodsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingMethodDto>>> GetShippingMethods()
        {
            var methods = await _context.ShippingMethods
                .Select(sm => new ShippingMethodDto
                {
                    ShippingMethodID = sm.ShippingMethodId,
                    MethodName = sm.MethodName,
                    DeliveryTime = sm.DeliveryTime
                })
                .ToListAsync();

            return Ok(methods);
        }

        [HttpPost]
        public async Task<ActionResult<ShippingMethodDto>> CreateShippingMethod(CreateShippingMethodRequest request)
        {
            var method = new ShippingMethod
            {
                MethodName = request.MethodName,
                DeliveryTime = request.DeliveryTime
            };

            _context.ShippingMethods.Add(method);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShippingMethods), null,
                new ShippingMethodDto
                {
                    ShippingMethodID = method.ShippingMethodId,
                    MethodName = method.MethodName,
                    DeliveryTime = method.DeliveryTime
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShippingMethod(int id, UpdateShippingMethodRequest request)
        {
            var method = await _context.ShippingMethods.FindAsync(id);
            if (method == null) return NotFound();

            method.MethodName = request.MethodName;
            method.DeliveryTime = request.DeliveryTime;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingMethod(int id)
        {
            var method = await _context.ShippingMethods.FindAsync(id);
            if (method == null) return NotFound();

            _context.ShippingMethods.Remove(method);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
