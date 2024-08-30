using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("editMyInfo")]
    [ApiController]
    public class EditInfoController:ControllerBase
    {
        //个人信息修改
        [HttpPost("editMyinfo")]
        public IActionResult EditMyinfo([FromBody] EditInfoViewModel query)
        {
            SqlSugarClient db = Database.GetInstance();
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            //获取token中的id
            try
            {
                if (query.NEW_NAME != null)
                {
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { NAME = query.NEW_NAME })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                if (query.NEW_SEX != null)
                {
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { NAME = query.NEW_SEX })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                if (query.NEW_ADDRESS != null)
                {
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { NAME = query.NEW_ADDRESS })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                return (Ok());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
