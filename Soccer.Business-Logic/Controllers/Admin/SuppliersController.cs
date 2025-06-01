using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly SoccerContext _context;

        public SuppliersController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
                .Select(s => new SupplierDto
                {
                    SupplierID = s.SupplierId,
                    SupplierName = s.SupplierName,
                    ContactInfo = s.ContactInfo
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDto>> CreateSupplier(CreateSupplierRequest request)
        {
            var supplier = new Supplier
            {
                SupplierName = request.SupplierName,
                ContactInfo = request.ContactInfo
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSuppliers), null,
                new SupplierDto
                {
                    SupplierID = supplier.SupplierId,
                    SupplierName = supplier.SupplierName,
                    ContactInfo = supplier.ContactInfo
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, UpdateSupplierRequest request)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            supplier.SupplierName = request.SupplierName;
            supplier.ContactInfo = request.ContactInfo;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
