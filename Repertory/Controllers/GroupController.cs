using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repertory.Services;

namespace Repertory.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase {

        private readonly GroupService _groupService;
        public GroupController(GroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        [Route("get_from_category/{id}")]
        public async Task<IActionResult> GetFromCategoryAsync(int id) {
            return Ok(await _groupService.GetFromCategoryAsync(id));
        }

        [HttpGet]
        [Route("get_composition/{id}")]
        public async Task<IActionResult> GetCompositionByIdAsync(long id) {
            return Ok(new {
                composition = await _groupService.GetCompositionByIdAsync(id)
            });
        }
    }
}
