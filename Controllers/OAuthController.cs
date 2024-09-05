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
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Xml.Linq;

namespace UniTrade.Controllers
{
    [Route("oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        // TODO: token 刷新，参考 https://www.cnblogs.com/l-monstar/p/17337768.html

        IPasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
        /*
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
        */
        /// <summary>
        /// 登录
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                USERS user;

                if (request.UseVerificationCode)
                {
                    user = db.Queryable<USERS>()
                    .Where(c => c.PHONE == request.name || c.EMAIL == request.name)
                    .First();



                    if (user == null)
                    {
                        return Unauthorized("用户不存在");
                    }
                    // 验证验证码
                    if (request.password != EmailController.LogVeriCode)
                    {
                        return Unauthorized("验证码无效");
                    }
                }
                else
                {
                    user = db.Queryable<USERS>()
                    .Where(c => c.NAME == request.name)
                    .First();


                    if (user == null)
                    {
                        return Unauthorized("用户不存在");
                    }
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
                var Token = JwtService.GenerateAccessToken(user_id, "User");
                var response = new
                {
                    access_token = Token,
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
        /// 注销
        /// </summary>
        [HttpPost("cancel")]
        public IActionResult CancelAccount([FromBody] CancelInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                var user = db.Queryable<USERS>()
                    .Where(c => c.USER_ID == userId)
                    .First();
                if (user == null)
                {
                    return Unauthorized("用户不存在");
                }

                // 验证密码是否正确（数据库中的密码是加密后的）
                var passwordVerification = passwordHasher.VerifyHashedPassword(
                        new IdentityUser(),
                        user.PASSWORD,
                        request.password
                        );
                if (passwordVerification != PasswordVerificationResult.Success)
                {
                    return BadRequest("密码错误，注销失败");
                }
                // 获取该用户的所有发售商品
                var sells = db.Queryable<SELLS>()
                    .Where(s => s.SELLER_ID == user.USER_ID)
                    .ToList();
                if (sells.Count > 0)
                {
                    var merchandiseIds = sells.Select(s => s.MERCHANDISE_ID).ToList();
                    // 删除发售商品
                    db.Deleteable<MERCHANDISES>()
                        .In(merchandiseIds)
                        .ExecuteCommand();
                    // 删除商品相关的图片
                    db.Deleteable<MERCHANDISES_PICTURE>()
                        .Where(p => merchandiseIds.Contains(p.MERCHANDISE_ID))
                        .ExecuteCommand();
                }

                db.Deleteable<USERS>()
                     .Where(u=>u.USER_ID==userId)
                     .ExecuteCommand();
                return Ok("账号注销成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
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

                // 生成时间戳唯一ID
                string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                Random random = new Random();
                string randomDigits = random.Next(1000, 9999).ToString(); // 四位随机数
                string uniqueId = timestamp + randomDigits; //时间戳加4位随机数组成最终id  例：202409051955000

                USERS newuser = new USERS
                {
                    USER_ID = uniqueId,
                    AVATAR = " ",
                    NAME = request.name,
                    PASSWORD = passwordHasher.HashPassword(new IdentityUser(), request.password),
                    PHONE = request.PhoneNumber,
                    ADDRESS = " ",
                    REPUTATION = 100,
                    CREDIT_NUMBER = " ",
                    SEX = " ",
                    EMAIL = request.Email
                };

                //插入表中
                db.Insertable(newuser).ExecuteCommand();

                return Ok("注册成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }
    }

    // 这里的 LoginRequest 移至 ViewModels 中变为 LoginInfoViewModel 了
    // 所有用于与前端交换数据用的实体类都放在 ViewModels 下
}
// vim: set sw=4:
