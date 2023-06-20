using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repertory.Repositories;
using Repertory.Services;
using Repertory.Utils;

namespace Repertory.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase {
        private readonly CategoryService _categoryService;
        public CategoryController(CategoryService categoryService) {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectedCategories(
            [FromQuery] bool? include_orchestra_or_ensemble,
            [FromQuery] int? nbr_instrumentalists,
            [FromQuery] int? page,
            [FromQuery] int? results_per_page,
            [FromQuery] string? instrument_families_to_include) {
            return Ok(await _categoryService.GetSelectedCategoriesAsync(
                include_orchestra_or_ensemble ?? false,
                nbr_instrumentalists ?? 1,
                page ?? 1,
                results_per_page ?? PaginationParameters.DEFAULT_RESULT,
                instrument_families_to_include ?? ""
                )
            );
        }
    }
}
