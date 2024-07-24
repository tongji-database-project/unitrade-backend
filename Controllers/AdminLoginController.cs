using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        [HttpPost]
        public IActionResult Login([FromBody] LoginInfoViewModel request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var adminstrator = db.Queryable<ADMINISTRATORS>()
                    .Where(a => a.ADMIN_NAME == request.name && a.ADMIN_PASSWORD == request.password)
                    .First();
                if (adminstrator != null)
                {
                    var token = JwtService.GenerateAccessToken(adminstrator.ADMIN_ID, "Admin");
                    return Ok(token);
                }
                else
                    return Unauthorized("用户名或密码错误");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
