using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResetPwdController : Controller
    {
        [HttpPatch]
        public async Task<IActionResult> ResetPassword([FromBody] resetPwdInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            IPasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();

            // 验证验证码
            if (request.VerificationCode != EmailController.FinVeriCode)
            {
                return Unauthorized("验证码无效");
            }

            var user = db.Queryable<USERS>()
                         .Where(c => c.NAME == request.name)
                         .First();

            if (user == null)
            {
                return NotFound("该用户不存在");
            }

            if(user.EMAIL != request.Email && user.PHONE != request.PhoneNumber)
            {
                return BadRequest("邮箱或手机号不匹配");
            }

            // 验证新密码与旧密码是否相同
            var passwordVerification = passwordHasher.VerifyHashedPassword(
                new IdentityUser(),
                user.PASSWORD,
                request.newPassword
            );

            if (passwordVerification == PasswordVerificationResult.Success)
            {
                return Unauthorized("新密码与旧密码相同");
            }

            string hashedNewPassword = passwordHasher.HashPassword(new IdentityUser(), request.newPassword);

            user.PASSWORD = hashedNewPassword;
            var updateResult = db.Updateable(user).Where(c => c.USER_ID == user.USER_ID).ExecuteCommand();

            if (updateResult > 0)
            {
                return Ok("密码重置成功");
            }
            else
            {
                return StatusCode(500, "密码重置失败");
            }
        }
    }
}
