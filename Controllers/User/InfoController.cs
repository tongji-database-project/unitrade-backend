using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SqlSugar;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("info")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserInfo>> Get()
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            // 根据 user_id 查询用户信息
            var userInfo = await db.Queryable<USERS>()
                .Where(it => it.USER_ID == userIdClaim)
                .FirstAsync();

            return Ok(new UserInfo(userInfo));
        }
    }
}
// vim: set sw=4:
