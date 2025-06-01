using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly SoccerContext _context;

        public SizesController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SizeDto>>> GetSizes()
        {
            var sizes = await _context.Sizes
                .OrderBy(s => s.SizeOrder)
                .Select(s => new SizeDto
                {
                    SizeID = s.SizeId,
                    SizeName = s.SizeName,
                    SizeOrder = s.SizeOrder
                })
                .ToListAsync();

            return Ok(sizes);
        }

        [HttpPost]
        public async Task<ActionResult<SizeDto>> CreateSize(CreateSizeRequest request)
        {
            var size = new Size
            {
                SizeName = request.SizeName,
                SizeOrder = request.SizeOrder
            };

            _context.Sizes.Add(size);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSizes), null,
                new SizeDto
                {
                    SizeID = size.SizeId,
                    SizeName = size.SizeName,
                    SizeOrder = size.SizeOrder
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSize(int id, UpdateSizeRequest request)
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size == null) return NotFound();

            size.SizeName = request.SizeName;
            size.SizeOrder = request.SizeOrder;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(int id)
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size == null) return NotFound();

            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
