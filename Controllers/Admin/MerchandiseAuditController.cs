using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Admin
{
    [Route("merchandiseaudit")]
    [ApiController]
    public class MerchandiseAuditController : ControllerBase
    {
        [HttpGet("query")]
        public IActionResult GetInfo()
        {
            return Ok();
        }

        [HttpPost("offshelf")]
        public IActionResult AuditRefunds([FromBody] string id)
        {
            return Ok();
        }
    }
}
