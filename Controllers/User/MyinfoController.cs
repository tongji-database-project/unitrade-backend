using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;




namespace UniTrade.Controllers.User
{
    [Route("getMyinfo")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetMyinfo()
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            try
            {
                var user = db.Queryable<USERS>()
                    .Where(a => a.USER_ID == userIdClaim)
                    .First();
                if(user == null)
                {
                    return Unauthorized("用户不存在");   /*暂定，用于测试，实际上线后按照*/
                }
                var back = new {
                    id = user.USER_ID,name = user.NAME,avatar=user.AVATAR,
                    address = user.ADDRESS,phone = user.PHONE,reputation = user.REPUTATION,
                    sex=user.SEX,email=user.EMAIL};
                return (Ok(back));
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
