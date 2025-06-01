using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class FieldsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public FieldsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FieldDto>>> GetFields()
        {
            var fields = await _context.Fields
                .Include(f => f.FieldImages)
                .Select(f => new FieldDto
                {
                    FieldID = f.FieldId,
                    FieldName = f.FieldName,
                    Location = f.Location,
                    PricePerHour = f.PricePerHour,
                    ImageUrls = f.FieldImages.Select(i => i.ImageUrl).ToList()
                })
                .ToListAsync();

            return Ok(fields);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FieldDto>> GetField(int id)
        {
            var field = await _context.Fields
                .Include(f => f.FieldImages)
                .FirstOrDefaultAsync(f => f.FieldId == id);

            if (field == null) return NotFound();

            return Ok(new FieldDto
            {
                FieldID = field.FieldId,
                FieldName = field.FieldName,
                Location = field.Location,
                PricePerHour = field.PricePerHour,
                ImageUrls = field.FieldImages.Select(i => i.ImageUrl).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<FieldDto>> CreateField(CreateFieldRequest request)
        {
            var field = new Field
            {
                FieldName = request.FieldName,
                Location = request.Location,
                PricePerHour = request.PricePerHour
            };

            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            // Add images if provided
            if (request.ImageUrls?.Any() == true)
            {
                var images = request.ImageUrls.Select(url => new FieldImage
                {
                    FieldId = field.FieldId,
                    ImageUrl = url
                }).ToList();

                _context.FieldImages.AddRange(images);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetField), new { id = field.FieldId },
                await GetField(field.FieldId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateField(int id, UpdateFieldRequest request)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field == null) return NotFound();

            field.FieldName = request.FieldName;
            field.Location = request.Location;
            field.PricePerHour = request.PricePerHour;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field == null) return NotFound();

            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
