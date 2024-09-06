using Microsoft.AspNetCore.Mvc;
using UniTrade.Tools;
using UniTrade.Models;
using UniTrade.ViewModels;
using SqlSugar;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace UniTrade.Controllers
{
    [Route("oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
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
                    return BadRequest("用户已存在");
                }

                var phone = db.Queryable<USERS>()
                    .Where(c => c.PHONE == request.PhoneNumber)
                    .First();
                if (phone != null)
                {
                    return BadRequest("该手机号已被占用");
                }

                var email = db.Queryable<USERS>()
                    .Where(c => c.EMAIL == request.Email)
                    .First();
                if (email != null)
                {
                    return BadRequest("该邮箱已被占用");
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
