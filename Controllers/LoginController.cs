using Microsoft.AspNetCore.Mvc;
using UniTrade.Tools;
using UniTrade.Models;
using SqlSugar;
using System.Reflection.Metadata.Ecma335;
namespace UniTrade.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {

            Console.WriteLine(loginRequest.username);
            Console.WriteLine(loginRequest.password);
            var result = loginRequest.username + loginRequest.password;
            return Ok(result);
        }
    }
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}