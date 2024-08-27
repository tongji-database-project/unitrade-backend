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
    [Route("adminjudge")]
    [ApiController]
    public class AdminJudgeController : ControllerBase
    {
        [HttpGet]
        public IActionResult CheckRole()
        {
            var userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            if(userRole== "Admin")
            {
                return Ok(true);
            }
            else
            {
                return Unauthorized(false);
            }
        }
    }
}
