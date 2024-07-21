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
        // TODO: token 刷新，参考 https://www.cnblogs.com/l-monstar/p/17337768.html

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
                if(customer != null || seller != null)
                {
                    string user_id = customer?.CUSTOMER_ID ?? seller.SELLER_ID;
                    var token = JwtService.GenerateAccessToken(user_id, "User");
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
    }

    public class LoginRequest
    {
        public string name { get; set; }
        public string password { get; set; }
    }
}



