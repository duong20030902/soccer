using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly BrandService _brandService;

        public BrandController(BrandService brandService)
        {
            _brandService = brandService;
        }

        // GET: /Admin/Brand
        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetBrandsAsync();
            return View(brands);
        }

        // POST: /Admin/Brand/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return BadRequest("Brand name cannot be empty.");

            var createdBrand = await _brandService.CreateBrandAsync(brandName);
            return Ok(createdBrand);
        }

        // PUT: /Admin/Brand/Edit/{id}
        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody] string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return BadRequest("Brand name cannot be empty.");

            var success = await _brandService.UpdateBrandAsync(id, brandName);
            return success ? Ok() : NotFound();
        }

        // DELETE: /Admin/Brand/Delete/{id}
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _brandService.DeleteBrandAsync(id);
            return success ? Ok() : NotFound();
        }
    }
}
