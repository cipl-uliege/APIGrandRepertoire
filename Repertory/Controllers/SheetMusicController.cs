using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repertory.Services;
using Repertory.Utils;

namespace Repertory.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SheetMusicController : ControllerBase {

        private readonly SheetMusicService _sheetMusicService;

        public SheetMusicController(SheetMusicService sheetMusicService) {
            _sheetMusicService = sheetMusicService;
        }

        [HttpGet]
        [Route("get_from_group/{id}")]
        public async Task<IActionResult> GetFromGroupAsync(
            long id,
            [FromQuery] int? page = null,
            [FromQuery] string? author = null,
            [FromQuery] string? title = null,
            int? results_per_page = null) {
            return Ok(await _sheetMusicService.GetFromGroupAsync(
                id,
                page ?? 1,
                author ?? "",
                title ?? "",
                results_per_page ?? PaginationParameters.DEFAULT_RESULT
                )
            );
        }

        [HttpGet]
        [Route("from_author_or_title")]
        public async Task<IActionResult> GetFromAuthorOrTitleAsync(
            [FromQuery] string? author,
            [FromQuery] int? nbr_instrumentalists,
            [FromQuery] string? title,
            [FromQuery] int? results_per_page,
            [FromQuery] int? page) {
            return Ok(await _sheetMusicService.GetFromAuthorOrTitleAsync(author ?? "", title ?? "", results_per_page ?? PaginationParameters.DEFAULT_RESULT, page ?? 1, nbr_instrumentalists ?? -1));
        }

        [HttpPost]
        [Route("get_favorites")]
        public async Task<IActionResult> GetFavoritesAsync(
            [FromBody] IEnumerable<int> favoriteIds,
            [FromQuery] string? author,
            [FromQuery] int? nbr_instrumentalists,
            [FromQuery] string? title,
            [FromQuery] int? page,
            [FromQuery] int? results_per_page) {

            if (favoriteIds.Count() > 100) {
                return BadRequest(new {
                    message = "The length of the list of favorites shouldn't exceed 100"
                });
            }

            return Ok(await _sheetMusicService.GetFavoritesAsync(favoriteIds.ToList(), page ?? 1, results_per_page ?? PaginationParameters.DEFAULT_RESULT, author ?? "", title ?? "", nbr_instrumentalists ?? -1));
        }
    }
}
