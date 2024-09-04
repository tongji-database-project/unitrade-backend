using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Admin
{
    [Route("admininfo")]
    [ApiController]
    public class AdminInfoController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAdminInfo()
        {
            SqlSugarClient db = Database.GetInstance();
            var role = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (role == "Admin")
            {
                try
                {
                    var admininfo = db.Queryable<ADMINISTRATORS>()
                        .Where(a => a.ADMIN_ID == id)
                        .Select(a => new AdminInfo
                        {
                            admin_id = a.ADMIN_ID,
                            admin_name = a.ADMIN_NAME,
                            admin_level = a.ADMIN_LEVEL,
                        })
                        .First();
                    if(admininfo != null)
                    {
                        return Ok(admininfo);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
