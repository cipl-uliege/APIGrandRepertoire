using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repertory.Services;
using Repertory.Utils;

namespace Repertory.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingLiteratureController : ControllerBase {

        private readonly TeachingLiteratureService _teachingLiteratureService;
        public TeachingLiteratureController(TeachingLiteratureService teachingLiteratureService) {
            _teachingLiteratureService = teachingLiteratureService;
        }

        [HttpGet]
        [Route("get_category")]
        public async Task<IActionResult> GetCategoryAndCountTitleAsync() {
            return Ok(await _teachingLiteratureService.GetCategoryAndCountTitle());
        }

        [HttpGet]
        [Route("get_from_group/{id}")]
        public async Task<IActionResult> GetFromGroupAsync(
            long id,
            [FromQuery] int? page = null,
            [FromQuery] string? author = null,
            [FromQuery] string? title = null,
            int? results_per_page = null) {
            return Ok(await _teachingLiteratureService.GetFromGroupAsync(id, page ?? 1, results_per_page ?? PaginationParameters.DEFAULT_RESULT, author ?? "", title ?? ""));
    }
}
}
