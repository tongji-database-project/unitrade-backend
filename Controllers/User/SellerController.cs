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
    [Route("api/seller")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        [Authorize]
        [HttpPost("publish")]
        public async Task<IActionResult> PublishProduct([FromBody] PublishProductViewModel model)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                //获取当前用户ID
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

                //验证用户是否存在
                var seller = db.Queryable<USERS>()
                    .Where(u => u.USER_ID == userId)
                    .First();
                if (seller == null)
                {
                    return Unauthorized("用户不存在");
                }

                //创建新的商品记录
                var newProduct = new MERCHANDISES
                {
                    MERCHANDISE_ID = Guid.NewGuid().ToString(),
                    MERCHANDISE_NAME = model.Name,
                    PRICE = model.Price,
                    INVENTORY = model.Inventory,
                    MERCHANDISE_TYPE = model.Type,
                    PICTURE_PATH = model.PicturePath
                };

                //保存商品到数据库
                await db.Insertable(newProduct).ExecuteCommandAsync();

                //建立卖家与商品的关系
                var sellsRecord = new SELLS
                {
                    MERCHANDISE_ID = newProduct.MERCHANDISE_ID,
                    SELLER_ID = seller.USER_ID
                };
                await db.Insertable(sellsRecord).ExecuteCommandAsync();

                return Ok(new { message = "商品发布成功", productId = newProduct.MERCHANDISE_ID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
