using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SqlSugar;
using UniTrade.Models;
using UniTrade.Tools;

namespace UniTrade.Controllers.User
{
    [Route("profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        public class UserInfo
        {
            public string Avatar { get; set; }
            public string Name { get; set; }
            public short Reputation { get; set; }

            public UserInfo() { }

            public UserInfo(USERS user)
            {
                Avatar = user.AVATAR;
                Name = user.NAME;
                Reputation = user.REPUTATION;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfo>> GetProfile(string id)
        {
            SqlSugarClient db = Database.GetInstance();

            // 根据路由传递的 id 查询用户信息
            var userInfo = await db.Queryable<USERS>()
                .Where(it => it.USER_ID == id)
                .FirstAsync();

            if (userInfo == null)
            {
                Console.WriteLine($"找不到用户{id}");
                return NotFound("用户不存在");
            }

            // 返回用户信息
            return Ok(new UserInfo
            {
                Avatar = userInfo.AVATAR,
                Name = userInfo.NAME,
                Reputation = userInfo.REPUTATION
            });
        }
    }
}
