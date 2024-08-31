using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Org.BouncyCastle.Asn1.Ocsp;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("editPassword")]
    [ApiController]
    public class EditPasswordController : ControllerBase
    {
        //个人信息修改
        [HttpPost]
        public IActionResult EditPassword([FromBody] EditPasswordViewModel query)
        {
            SqlSugarClient db = Database.GetInstance();
            IPasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            //获取token中的id
            try
            {
                var password = db.Queryable<USERS>()  //根据id来获取原密码
                    .Where(u => u.USER_ID == userIdClaim)
                         .Select(u => u.PASSWORD)
                         .Single();
                if (query.ORIGIN_PASSWORD == password)  //验证密码是否正确
                {
                    if (query.CONFIRM_PASSWORD == query.NEW_PASSWORD)  //验证两次输入的密码是否相同
                    {
                        query.NEW_PASSWORD = passwordHasher.HashPassword(new IdentityUser(), query.NEW_PASSWORD);
                        db.Updateable<USERS>()
                            .SetColumns(u => new USERS { PASSWORD = query.NEW_PASSWORD })
                            .Where(u => u.USER_ID == userIdClaim)
                            .ExecuteCommand();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("两次密码不一致");
                    }
                }
                else
                {
                    return BadRequest("原密码输入错误");
                }
                return (Ok());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
