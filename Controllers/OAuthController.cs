using Microsoft.AspNetCore.Mvc;
using UniTrade.Tools;
using UniTrade.Models;
using UniTrade.ViewModels;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Runtime.CompilerServices;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace UniTrade.Controllers
{
    [Route("oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        // TODO: token 刷新，参考 https://www.cnblogs.com/l-monstar/p/17337768.html

        IPasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();

        /// <summary>
        /// 登录
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var user = db.Queryable<USERS>()
                    .Where(c => c.NAME == request.name)
                    .First();

                if (user == null)
                {
                    return Unauthorized("用户不存在");
                }

                // 加密密码的生成方式（"password" 为密码字符串）：
                // var hashedPassword = passwordHasher.HashPassword(new IdentityUser(), "password");

                // 验证密码是否正确（数据库中的密码是加密后的）
                var passwordVerification = passwordHasher.VerifyHashedPassword(
                        new IdentityUser(),
                        user.PASSWORD,
                        request.password
                        );
                if (passwordVerification != PasswordVerificationResult.Success)
                {
                    return Unauthorized("密码错误");
                }

                // 密码正确则生成 token 并返回
                string user_id = user.USER_ID;
                var token = JwtService.GenerateAccessToken(user_id, "User");
                var back = new { token = token, id = user_id };
                return Ok(back);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //注册
        [HttpPost("register")]
        public IActionResult register([FromBody] RegisterInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var user = db.Queryable<USERS>()
                    .Where(c => c.NAME == request.name)
                    .First();

                if (user != null)
                {
                    return Unauthorized("用户已存在");
                }

                int sum = db.Queryable<USERS>().Count() + 1;
                //生成ID
                string id = sum.ToString("D20");

                USERS newuser = new USERS();
                newuser.USER_ID = id;
                newuser.NAME = request.name;
                //加密存储
                newuser.PASSWORD= passwordHasher.HashPassword(new IdentityUser(), request.password); 
                newuser.REPUTATION = 100;

                //插入表中
                db.Insertable(newuser).ExecuteCommand();

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // 这里的 LoginRequest 移至 ViewModels 中变为 LoginInfoViewModel 了
    // 所有用于与前端交换数据用的实体类都放在 ViewModels 下
}
// vim: set sw=4:
