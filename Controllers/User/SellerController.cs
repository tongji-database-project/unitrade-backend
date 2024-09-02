using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("seller")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        //发布商品的api
        [Authorize]
        [HttpPost("publish")]
        public async Task<IActionResult> PublishProduct([FromBody] PublishProductViewModel model)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                //获取当前用户ID
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

                Console.Write("errortest0");

                //验证用户是否存在
                var seller = db.Queryable<USERS>()
                    .Where(u => u.USER_ID == userId)
                    .First();

                Console.Write("errortest1");


                if (seller == null)
                {
                    Console.Write("errortest2");
                    return Unauthorized("用户不存在");
                }

                // 保存图片到本地
                var fileName = $"{Guid.NewGuid()}_{model.Picture.FileName}";
                var filePath = Path.Combine(@"C:\data\images\cover", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Picture.CopyToAsync(stream);
                }
                Console.Write("errortest3");

                //创建新的商品记录
                var newProduct = new MERCHANDISES
                {
                    MERCHANDISE_ID = Guid.NewGuid().ToString(),
                    MERCHANDISE_NAME = model.Name,
                    PRICE = model.Price,
                    INVENTORY = model.Inventory,
                    MERCHANDISE_TYPE = model.Type,
                    COVER_PICTURE_PATH = $"/images/cover/{fileName}" // 保存相对路径到数据库
                };

                Console.Write("errortest4");
                //保存商品到数据库
                await db.Insertable(newProduct).ExecuteCommandAsync();
                db.Aop.OnLogExecuting = (sql, parameters) =>
                {
                    Console.WriteLine("SQL: " + sql);
                    Console.WriteLine("Parameters: " + string.Join(", ", parameters.Select(p => $"{p.ParameterName}: {p.Value}")));
                };
                Console.Write("errortest5");
                //建立卖家与商品的关系
                var sellsRecord = new SELLS
                {
                    MERCHANDISE_ID = newProduct.MERCHANDISE_ID,
                    SELLER_ID = seller.USER_ID
                };
                Console.Write("errortest6");
                //保存卖家与商品的关系到数据库
                await db.Insertable(sellsRecord).ExecuteCommandAsync();
                Console.Write("errortest7");
                return Ok(new { message = "商品发布成功", productId = newProduct.MERCHANDISE_ID });
            }
            catch (Exception ex)
            {
                Console.Write("errortest10");
                Console.WriteLine(ex.Message);
                db.Aop.OnLogExecuting = (sql, parameters) =>
                {
                    Console.WriteLine("SQL: " + sql);
                    Console.WriteLine("Parameters: " + string.Join(", ", parameters.Select(p => $"{p.ParameterName}: {p.Value}")));
                };
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //上传商品图片的api
        [HttpPost("sendPicture")]
        public async Task<IActionResult> SendProductPicture(IFormFile file)
        {
            Console.Write("errortest11");
            // 检查文件是否为空
            if (file == null || file.Length == 0)
            {
                return BadRequest("没有选择文件上传");
            }
            Console.Write("errortest12");
            // 定义文件保存路径
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(@"C:\data\images\cover", fileName);
            Console.Write("errortest13");
            try
            {
                // 保存文件到指定路径
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Console.Write("errortest14");
                    await file.CopyToAsync(stream);
                }

                // 返回成功响应，并将文件路径返回给前端
                Console.Write("errortest15");
                return Ok(new { url = $"/uploads/{fileName}" });
            }
            catch (Exception ex)
            {
                // 处理可能发生的异常
                Console.Write("errortest16");
                return StatusCode(500, $"文件上传失败: {ex.Message}");
            }
        }

    }
}
