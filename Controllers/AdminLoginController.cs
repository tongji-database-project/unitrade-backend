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

namespace UniTrade.Controllers
{
    [Route("adminlogin")]
    [ApiController]
    public class AdminLoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var adminstrator = db.Queryable<ADMINISTRATORS>()
                    .Where(a => a.ADMIN_NAME == request.name && a.ADMIN_PASSWORD == request.password)
                    .First();
                if (adminstrator != null)
                {
                    var token = GenerateJwtToken(adminstrator.ADMIN_ID, "Admin");
                    return Ok(token);
                }
                else
                    return Unauthorized("用户名或密码错误");
            }
            catch (Exception ex)
            {
                return StatusCode(566, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateJwtToken(string id, string role)
        {

            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, id),
                new Claim(ClaimTypes.Role, role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenParameter.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            var token = new JwtSecurityToken(TokenParameter.Issuer,
              TokenParameter.Audience,
              claims,
              expires: DateTime.Now.AddMinutes(TokenParameter.TokenExpiry),
              signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }
    }
}
