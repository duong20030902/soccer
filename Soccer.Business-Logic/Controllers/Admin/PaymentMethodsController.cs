using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public PaymentMethodsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
        {
            var methods = await _context.PaymentMethods
                .Select(pm => new PaymentMethodDto
                {
                    PaymentMethodID = pm.PaymentMethodId,
                    MethodName = pm.MethodName
                })
                .ToListAsync();

            return Ok(methods);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentMethodDto>> CreatePaymentMethod([FromBody] string methodName)
        {
            var method = new PaymentMethod { MethodName = methodName };
            _context.PaymentMethods.Add(method);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPaymentMethods), null,
                new PaymentMethodDto { PaymentMethodID = method.PaymentMethodId, MethodName = method.MethodName });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaymentMethod(int id, [FromBody] string methodName)
        {
            var method = await _context.PaymentMethods.FindAsync(id);
            if (method == null) return NotFound();

            method.MethodName = methodName;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var method = await _context.PaymentMethods.FindAsync(id);
            if (method == null) return NotFound();

            _context.PaymentMethods.Remove(method);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
