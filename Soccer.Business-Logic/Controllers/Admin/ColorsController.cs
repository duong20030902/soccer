using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public ColorsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColorDto>>> GetColors()
        {
            var colors = await _context.Colors
                .Select(c => new ColorDto
                {
                    ColorID = c.ColorId,
                    ColorName = c.ColorName,
                    ColorCode = c.ColorCode
                })
                .ToListAsync();

            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColorDto>> GetColor(int id)
        {
            var color = await _context.Colors
                .Where(c => c.ColorId == id)
                .Select(c => new ColorDto
                {
                    ColorID = c.ColorId,
                    ColorName = c.ColorName,
                    ColorCode = c.ColorCode
                })
                .FirstOrDefaultAsync();

            if (color == null)
            {
                return NotFound();
            }

            return Ok(color);
        }

        [HttpPost]
        public async Task<ActionResult<ColorDto>> CreateColor(CreateColorRequest request)
        {
            var color = new Color
            {
                ColorName = request.ColorName,
                ColorCode = request.ColorCode
            };

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetColors), null,
                new ColorDto
                {
                    ColorID = color.ColorId,
                    ColorName = color.ColorName,
                    ColorCode = color.ColorCode
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColor(int id, UpdateColorRequest request)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return NotFound();

            color.ColorName = request.ColorName;
            color.ColorCode = request.ColorCode;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return NotFound();

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
