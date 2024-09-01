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
        /// token刷新
        /// </summary>
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                // 从 RefreshToken 表中查找刷新令牌
                var refreshToken = db.Queryable<RefreshToken>()
                    .Where(rt => rt.Token == request.RefreshToken && !rt.IsRevoked)
                    .First();

                if (refreshToken == null)
                {
                    return Unauthorized("无效的刷新令牌");
                }

                // 检查刷新令牌是否过期
                if (refreshToken.Expiration <= DateTime.UtcNow)
                {
                    return Unauthorized("刷新令牌已过期");
                }

                // 生成新的访问令牌
                string userId = refreshToken.UserId;
                var newAccessToken = JwtService.GenerateAccessToken(userId, "User");

                // 生成新的刷新令牌并更新表
                var newRefreshToken = JwtService.GenerateRefreshToken();
                refreshToken.Token = newRefreshToken;
                refreshToken.Expiration = DateTime.UtcNow.AddDays(7); // 设定刷新令牌的有效期
                db.Updateable(refreshToken).ExecuteCommand();

                var response = new
                {
                    access_token = newAccessToken,
                    refresh_token = newRefreshToken
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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

                if (request.UseVerificationCode)
                {
                    // 验证验证码
                    if (request.password != EmailController.LogVeriCode)
                    {
                        return Unauthorized("验证码无效");
                    }
                }
                else
                {
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
                }

                // 密码或验证码正确则生成 token 并返回
                string user_id = user.USER_ID;
                var accessToken = JwtService.GenerateAccessToken(user_id, "User");
                var refreshToken = JwtService.GenerateRefreshToken();

                // 将刷新令牌保存到 RefreshToken 表
                db.Insertable(new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user_id,
                    Expiration = DateTime.UtcNow.AddDays(7) // 设置刷新令牌的过期时间
                }).ExecuteCommand();

                var response = new
                {
                    access_token = accessToken,
                    refresh_token = refreshToken,
                    id = user_id
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                // 撤销当前的刷新令牌
                db.Updateable<RefreshToken>()
                    .SetColumns(rt => new RefreshToken { IsRevoked = true })
                    .Where(rt => rt.Token == request.RefreshToken)
                    .ExecuteCommand();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        [HttpPost("register")]
        public IActionResult register([FromBody] RegisterInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                // 验证验证码
                if (request.VerificationCode != EmailController.RegVeriCode)
                {
                    return Unauthorized("验证码无效");
                }

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

                USERS newuser = new USERS
                {
                    USER_ID = id,
                    NAME = request.name,
                    //加密存储
                    PASSWORD = passwordHasher.HashPassword(new IdentityUser(), request.password),
                    REPUTATION = 100,
                    PHONE = request.PhoneNumber,
                    EMAIL = request.Email,
                };
               
                //插入表中
                db.Insertable(newuser).ExecuteCommand();

                return Ok("注册成功");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }
    }

    // 这里的 LoginRequest 移至 ViewModels 中变为 LoginInfoViewModel 了
    // 所有用于与前端交换数据用的实体类都放在 ViewModels 下
}
// vim: set sw=4:
