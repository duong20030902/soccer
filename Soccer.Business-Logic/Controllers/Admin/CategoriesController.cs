using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly SoccerContext _context;

        public CategoriesController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Parent)
                .Select(c => new CategoryDto
                {
                    CategoryID = c.CategoryId,
                    CategoryName = c.CategoryName,
                    ParentID = c.ParentId,
                    ParentName = c.Parent != null ? c.Parent.CategoryName : null
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Parent)
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryID = c.CategoryId,
                    CategoryName = c.CategoryName,
                    ParentID = c.ParentId,
                    ParentName = c.Parent != null ? c.Parent.CategoryName : null
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryRequest request)
        {
            var category = new Category
            {
                CategoryName = request.CategoryName,
                ParentId = request.ParentID
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), null,
                new CategoryDto
                {
                    CategoryID = category.CategoryId,
                    CategoryName = category.CategoryName,
                    ParentID = category.ParentId
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.CategoryName = request.CategoryName;
            category.ParentId = request.ParentID;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
