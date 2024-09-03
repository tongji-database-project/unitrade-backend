using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SqlSugar;
using System;
using UniTrade.Models;
using UniTrade.ViewModels;
using UniTrade.Tools;
using System.Security.Claims;

namespace UniTrade.Controllers.Admin
{
    [Route("rolejudge")]
    [ApiController]
    public class RoleJudgeController : ControllerBase
    {
        [HttpGet]
        public IActionResult CheckRole()
        {
            var userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            if(userRole== "Admin")
            {
                return Ok("Admin");
            }
            else if(userRole == "User")
            {
                return Ok("User");
            }
            else
            {
                return Ok("Visitor");
            }
        }
    }
}
