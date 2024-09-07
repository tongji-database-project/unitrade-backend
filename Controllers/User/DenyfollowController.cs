using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("denyfollow")]
    [ApiController]
    public class DenyfollowController:ControllerBase
    {
        [HttpPost]
        public IActionResult Denyfollow([FromBody] DenyfollowViewModel query)
        {
            SqlSugarClient db = Database.GetInstance();
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            //获取token中的id
            try
            {
                Console.WriteLine("Asdasd");
                Console.WriteLine(userIdClaim);
                db.Deleteable<FOLLOWS>()
                     .Where(u => u.SELLER_ID==query.seller_id)
                     .ExecuteCommand();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
