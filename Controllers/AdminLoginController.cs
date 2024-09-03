using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers
{
    [Route("adminlogin")]
    [ApiController]
    public class AdminLoginController : ControllerBase
    {
        IPasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();

        [HttpPost]
        public IActionResult Login([FromBody] AdminLoginInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var adminstrator = db.Queryable<ADMINISTRATORS>()
                    .Where(a => a.ADMIN_NAME == request.name)// && a.ADMIN_PASSWORD == request.password)
                    .First();

                if (adminstrator == null)
                {
                    return Unauthorized("用户不存在");
                }

                // 验证密码是否正确（数据库中的密码为加密后的）
                var passwordVerification = passwordHasher.VerifyHashedPassword(
                    new IdentityUser(),
                    adminstrator.ADMIN_PASSWORD,
                    request.password
                    );
                if (passwordVerification != PasswordVerificationResult.Success)
                {
                    return Unauthorized("密码错误");
                }

                var token = JwtService.GenerateAccessToken(adminstrator.ADMIN_ID, "Admin");
                var back = new { token = token, id = adminstrator.ADMIN_ID };
                return Ok(back);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
