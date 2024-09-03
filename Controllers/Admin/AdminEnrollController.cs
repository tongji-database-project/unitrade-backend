using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SqlSugar;
using System;
using UniTrade.Models;
using UniTrade.ViewModels;
using UniTrade.Tools;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace UniTrade.Controllers.Admin
{
    [Route("adminenroll")]
    [ApiController]
    public class AdminEnrollController : ControllerBase
    {
        [HttpPost]
        public IActionResult AdminEnroll([FromBody] AdminEnrollInfo request)
        {
            IPasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            var adminId = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var AdminInfo = db.Queryable<ADMINISTRATORS>()
                    .Where(a => a.ADMIN_ID == adminId)
                    .First();
                if(AdminInfo != null)
                {
                    if (AdminInfo.ADMIN_LEVEL)
                    {
                        var Admin = db.Queryable<ADMINISTRATORS>()
                        .Where(a => a.ADMIN_NAME == request.name)
                        .First();

                        if (Admin != null)
                        {
                            return Unauthorized("管理员已存在");
                        }
                        else
                        {
                            int sum = db.Queryable<ADMINISTRATORS>().Count() + 1;
                            string id = sum.ToString("D20");
                            ADMINISTRATORS newadmin = new ADMINISTRATORS();
                            newadmin.ADMIN_ID = id;
                            newadmin.ADMIN_NAME = request.name;
                            newadmin.ADMIN_PASSWORD = passwordHasher.HashPassword(new IdentityUser(), request.password);
                            if (request.level == "1")
                            {
                                newadmin.ADMIN_LEVEL = true;
                            }
                            else
                            {
                                newadmin.ADMIN_LEVEL = false;
                            }
                            db.Insertable(newadmin).ExecuteCommand();
                            return Ok();
                        }
                    }

                    else
                    {
                        return Unauthorized("没有权限注册新的管理员");
                    }
                }
                else
                {
                    return Unauthorized("管理员不存在");
                }
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}