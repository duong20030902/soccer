using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public BrandsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brands = await _context.Brands
                .Select(b => new BrandDto
                {
                    BrandID = b.BrandId,
                    BrandName = b.BrandName
                })
                .ToListAsync();

            return Ok(brands);
        }

        [HttpPost]
        public async Task<ActionResult<BrandDto>> CreateBrand([FromBody] string brandName)
        {
            var brand = new Brand { BrandName = brandName };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBrands), null,
                new BrandDto { BrandID = brand.BrandId, BrandName = brand.BrandName });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] string brandName)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            brand.BrandName = brandName;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
