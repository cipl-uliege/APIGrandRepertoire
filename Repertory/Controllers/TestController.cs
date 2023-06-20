using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Repertory.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase {

        private SqlConnection _conn;
        public TestController(SqlConnection connection)
        {
            _conn = connection;
        }

        [HttpGet]
        public IActionResult ResponseTest() {
            try {
                int nbr = 0;
                using (_conn) {
                    _conn.Open();
                    using(SqlCommand command = new SqlCommand("SELECT 24", _conn)) {
                        nbr = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                return Ok(new {
                    msg = "connection successfull to the db" + nbr
                });
            }catch(Exception e) {
                return Ok(new {
                    msg = "catch block " + e.Message
                });
            }
        }

        [HttpGet]
        [Route("response")]
        public IActionResult ResponseTestRoute() {
            return Ok(new {
                msg = "success"
            });
        }
    }
}
