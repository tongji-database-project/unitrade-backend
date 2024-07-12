using Microsoft.AspNetCore.Mvc;
using UniTrade.Tools;
using UniTrade.Models;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Runtime.CompilerServices;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace UniTrade.Controllers
{
    [Route("oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        /*private readonly SqlSugarClient _db;

        public OAuthController(SqlSugarClient db)
        {
            _db = db;
        }*/

        //

        //登录
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {

            SqlSugarClient db = Database.GetInstance();
            try
            {
                var customer = db.Queryable<CUSTOMERS>()
                    .Where(c => c.CUSTOMER_NAME == request.name && c.CUSTOMER_PASSWORD == request.password)
                    .First();
                var seller = db.Queryable<SELLERS>()
                    .Where(s => s.SELLER_NAME == request.name && s.SELLER_PASSWORD == request.password)
                    .First();
                var adminstrator = db.Queryable<ADMINISTRATORS>()
                    .Where(a=>a.ADMIN_NAME == request.name && a.ADMIN_PASSWORD == request.password)
                    .First();

                if(adminstrator != null)
                {
                    var token = GenerateJwtToken(adminstrator.ADMIN_ID, "Admin");
                    return Ok(token);
                }
                else if(customer != null || seller != null)
                {
                    //var user_name = customer.CUSTOMER_NAME != null ? customer.CUSTOMER_NAME : seller.SELLER_NAME;
                    //string user_name = customer?.CUSTOMER_NAME ?? seller.SELLER_NAME;
                    string user_id = customer?.CUSTOMER_ID ?? seller.SELLER_ID;

                    var token = GenerateJwtToken(user_id, "User");
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


        //注册
        //[HttpPost("register")]
        ////.........


        private string GenerateJwtToken(string id,string role)
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

    public class LoginRequest
    {
        public string name { get; set; }
        public string password { get; set; }
    }
}



