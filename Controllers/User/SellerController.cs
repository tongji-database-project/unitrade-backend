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

                Console.Write("errortest3");

                //创建新的商品记录
                var newProduct = new MERCHANDISES
                {
                    MERCHANDISE_ID = Guid.NewGuid().ToString(),
                    MERCHANDISE_NAME = model.name,
                    PRICE = model.price,
                    INVENTORY = model.inventory,
                    MERCHANDISE_TYPE = model.type,
                    COVER_PICTURE_PATH = model.cover_image_url,
                    DETAILS = model.product_details
                };
                await db.Insertable(newProduct).ExecuteCommandAsync();
                //创建商品与卖家的关系
                var newSellsRecord = new SELLS
                {
                    MERCHANDISE_ID = newProduct.MERCHANDISE_ID,
                    SELLER_ID = seller.USER_ID
                };
                await db.Insertable(newSellsRecord).ExecuteCommandAsync();
                //创建商品详情图片与商品的关系
                foreach (var imageUrl in model.product_image_urls)
                {
                    var newDetailsPicture = new MERCHANDISES_PICTURE
                    {
                        MERCHANDISE_ID = newProduct.MERCHANDISE_ID,
                        PICTURE_PATH = imageUrl
                    };
                    await db.Insertable(newDetailsPicture).ExecuteCommandAsync();
                }
                return Ok();
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

        //上传商品详情图片的api
        [HttpPost("sendDetails")]
        public async Task<IActionResult> SendProductDetails(IFormFile file)
        {
            Console.Write("errortest11");
            try
            {
                // 调用 StoreImage 工具类保存图片，指定 API 类型为 "productDetails"
                var imageUrl = await StoreImage.SaveImageAsync(file, "productDetails");

                Console.Write("errortest15");
                // 返回成功响应，并将文件路径返回给前端
                return Ok(new { url = imageUrl });
            }
            catch (ArgumentException argEx)
            {
                // 处理文件为空或无效的情况
                Console.Write("errortest12");
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                // 处理可能发生的异常
                Console.Write("errortest16");
                return StatusCode(500, $"文件上传失败: {ex.Message}");
            }
        }


        //上传商品封面图片的api
        [HttpPost("sendCover")]
        public async Task<IActionResult> SendProductCover(IFormFile file)
        {
            Console.Write("errortest11");
            try
            {
                // 调用 StoreImage 工具类保存图片，指定 API 类型为 "cover"
                var imageUrl = await StoreImage.SaveImageAsync(file, "cover");

                Console.Write("errortest15");
                // 返回成功响应，并将文件路径返回给前端
                return Ok(new { url = imageUrl });
            }
            catch (ArgumentException argEx)
            {
                // 处理文件为空或无效的情况
                Console.Write("errortest12");
                return BadRequest(argEx.Message);
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
