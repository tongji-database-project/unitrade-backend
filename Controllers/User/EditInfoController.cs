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
                if (query.NEW_NAME!="")  //对名称进行修改
                {
                    is_edit = true;
                    var name = db.Queryable<USERS>().
                        Where(u => u.USER_ID == userIdClaim).
                        Select(u => u.NAME).Single();
                    if(name==query.NEW_NAME)
                    {
                        return BadRequest("与原名字相同");
                    }
                    bool exists = db.Queryable<USERS>()
                        .Any(u => u.NAME == query.NEW_NAME);
                    if(exists)
                    {
                        return BadRequest("已存在该名称");
                    }
                    else
                    {
                        db.Updateable<USERS>()
                            .SetColumns(u => new USERS { NAME = query.NEW_NAME })
                            .Where(u => u.USER_ID == userIdClaim)
                            .ExecuteCommand();
                    }
                }
                if (query.NEW_SEX != "")
                {
                    is_edit = true;
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
                    return (BadRequest("无输入"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}