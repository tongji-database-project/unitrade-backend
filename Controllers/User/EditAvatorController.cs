using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Org.BouncyCastle.Asn1.Ocsp;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections;

namespace UniTrade.Controllers.User
{
    [ApiController]
    [Route("setpicture")]
    public class EditAvatorController : ControllerBase 
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "img/avator");  /*上传到服务器的路径*/

        public EditAvatorController()
        {
            // 第一次使用就创建一个文件夹
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }
        [HttpPost]
        public async Task<IActionResult> editAvator(IFormFile file)
        {
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            SqlSugarClient db = Database.GetInstance();
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // 创建文件路径
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_uploadPath, fileName);

                //保存到服务器
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                db.Updateable<USERS>()
                          .SetColumns(u => new USERS { AVATAR = fileName })
                          .Where(u => u.USER_ID == userIdClaim)
                          .ExecuteCommand(); 

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
