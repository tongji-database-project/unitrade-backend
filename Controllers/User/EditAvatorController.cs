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
    [ApiController]
    [Route("setpicture")]
    public class EditAvatorController : ControllerBase 
    {
        private readonly string _uploadPath = Path.Combine("C:/data/images");  /*上传到服务器的路径*/


        public EditAvatorController()
        {
            // 第一次使用就创建一个文件夹
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }
        [HttpPost]
        public async Task<IActionResult> editAvator(IFormFile File)
        {
            SqlSugarClient db = Database.GetInstance();
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            try
            {
                if (File == null ||File.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // 创建文件路径
                var fileName = Path.GetFileName(File.FileName);
                var filePath = Path.Combine(_uploadPath, fileName);
                //保存到服务器
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
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
