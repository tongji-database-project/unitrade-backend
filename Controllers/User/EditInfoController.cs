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
        [HttpPost]
        public IActionResult EditMyinfo([FromBody] EditInfoViewModel query)
        {
            SqlSugarClient db = Database.GetInstance();
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            bool is_edit = false;
            //获取token中的id
            try
            {
                if (query.NEW_NAME!="")
                {
                    is_edit = true;
                    Console.WriteLine(query.NEW_NAME.Length);
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { NAME = query.NEW_NAME })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                if (query.NEW_SEX != "")
                {
                    is_edit = true;
                    Console.WriteLine("SEX");
                    Console.WriteLine(query.NEW_SEX);
                    var new_sex = "";
                    if (query.NEW_SEX == "男")
                    {
                        new_sex = "m";
                    }
                    if (query.NEW_SEX == "女")
                    {
                        new_sex = "f";
                    }
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { SEX = new_sex })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                if (query.NEW_ADDRESS != "")
                {
                    is_edit = true;
                    Console.WriteLine("ADDRESS");
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { ADDRESS = query.NEW_ADDRESS })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                if(is_edit)
                {
                    return (Ok());
                }
                else
                {
                    return (Ok());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
/*
                 if (query.NEW_NAME != null)
                {
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { NAME=query.NEW_NAME})
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                if (query.NEW_SEX != null)
                {
                    var new_sex = "";
                    if(query.NEW_SEX=="男")
                    {
                        new_sex = "m";
                    }
                    if (query.NEW_SEX == "女")
                    {
                        new_sex = "f";
                    }
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { SEX = new_sex })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
                if (query.NEW_ADDRESS != null)
                {
                    db.Updateable<USERS>()
                        .SetColumns(u => new USERS { ADDRESS = query.NEW_ADDRESS })
                        .Where(u => u.USER_ID == userIdClaim)
                        .ExecuteCommand();
                }
 */