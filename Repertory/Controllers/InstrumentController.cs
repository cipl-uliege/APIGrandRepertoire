using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repertory.Repositories;
using Repertory.Services;
using Repertory.Utils;

namespace Repertory.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentController : ControllerBase {

        private readonly InstrumentService _instrumentService;
        public InstrumentController(InstrumentService instrumentService) {
            _instrumentService = instrumentService;
        }

        [HttpGet]
        [Route("get_name_from_composition/{composition}")]
        public async Task<IActionResult> GetFullNameOfComposition(string composition) {
            return Ok(await _instrumentService.GetFullNameOfCompositionAsync(composition));
        }
    }
}
