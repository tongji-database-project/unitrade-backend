using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Security.Claims;
using System.Security.Principal;
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
                //验证用户是否存在
                var seller = db.Queryable<USERS>()
                    .Where(u => u.USER_ID == userId)
                    .First();
                if (seller == null)
                {
                    Console.Write("errortest2");
                    return Unauthorized("用户不存在");
                }

                //创建新的商品记录
                var newProduct = new MERCHANDISES
                {
                    MERCHANDISE_ID = Guid.NewGuid().ToString(),
                    MERCHANDISE_NAME = model.name,
                    PRICE = model.price*100,
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
            try
            {
                // 调用 StoreImage 工具类保存图片，指定 API 类型为 "productDetails"
                var imageUrl = await StoreImage.SaveImageAsync(file, "productDetails");

                // 返回成功响应，并将文件路径返回给前端
                return Ok(new { url = imageUrl });
            }
            catch (ArgumentException argEx)
            {
                // 处理文件为空或无效的情况
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                // 处理可能发生的异常
                return StatusCode(500, $"文件上传失败: {ex.Message}");
            }
        }


        //上传商品封面图片的api
        [HttpPost("sendCover")]
        public async Task<IActionResult> SendProductCover(IFormFile file)
        {
            try
            {
                // 调用 StoreImage 工具类保存图片，指定 API 类型为 "cover"
                var imageUrl = await StoreImage.SaveImageAsync(file, "cover");

                // 返回成功响应，并将文件路径返回给前端
                return Ok(new { url = imageUrl });
            }
            catch (ArgumentException argEx)
            {
                // 处理文件为空或无效的情况
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                // 处理可能发生的异常
                return StatusCode(500, $"文件上传失败: {ex.Message}");
            }
        }

        //获取某用户所发布商品信息的api
        [Authorize]
        [HttpGet("getUserProducts")]
        public async Task<IActionResult> GetUserProducts()
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
                // 获取用户发布的商品ID
                var merchandiseIds = db.Queryable<SELLS>()
                    .Where(s => s.SELLER_ID == userId)
                    .Select(s => s.MERCHANDISE_ID)
                    .ToList();
                // 获取商品信息
                var products = db.Queryable<MERCHANDISES>()
                    .Where(m => merchandiseIds.Contains(m.MERCHANDISE_ID))
                    .Select(product => new
                    {
                        product.MERCHANDISE_ID,
                        product.MERCHANDISE_NAME,
                        product.PRICE,
                        product.INVENTORY,
                        product.MERCHANDISE_TYPE,
                        product.COVER_PICTURE_PATH,
                        product.DETAILS
                    })
                    .ToList();

                // 从 ORDERS 表中统计每个商品的销量
                var salesData = db.Queryable<ORDERS>()
                    .Where(o => merchandiseIds.Contains(o.MERCHANDISE_ID))
                    .GroupBy(o => o.MERCHANDISE_ID)
                    .Select(o => new
                    {
                        MerchandiseId = o.MERCHANDISE_ID,
                        Sales = SqlFunc.AggregateSum(o.ORDER_QUANITY) // 使用聚合函数计算销量总和
                    })
                    .ToList();

                // 合并销量数据到商品信息中
                var productsWithSales = products.Select(product => new GetSellerProductsViewModels(
                    product.MERCHANDISE_ID, // 这里的命名应与类的属性一致
                    product.MERCHANDISE_NAME,
                    product.PRICE,
                    product.INVENTORY,
                    product.MERCHANDISE_TYPE,
                    product.COVER_PICTURE_PATH,
                    product.DETAILS,
                    salesData.FirstOrDefault(s => s.MerchandiseId == product.MERCHANDISE_ID)?.Sales ?? 0
                )).ToList();

                // 返回商品信息
                return Ok(productsWithSales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //更改发布商品信息的api
        [Authorize]
        [HttpPost("modify")]
        public async Task<IActionResult> ModifyProduct([FromBody] ProductIDViewModel model)
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

                var productID = model.ProductID;

                //获取商品的详细信息
                var theProduct = db.Queryable<MERCHANDISES>()
                    .Where(u => u.MERCHANDISE_ID == productID)
                    .First();

                if (theProduct == null)
                {
                    return NotFound("商品不存在");
                }

                //获取商品的详细图片信息
                var thePictures = db.Queryable<MERCHANDISES_PICTURE>()
                    .Where(m => m.MERCHANDISE_ID == productID)
                    .Select(m => m.PICTURE_PATH)
                    .ToList();

                var products = new PublishProductViewModel
                {
                    name = theProduct.MERCHANDISE_NAME,
                    price = theProduct.PRICE,
                    inventory = theProduct.INVENTORY,
                    type = theProduct.MERCHANDISE_TYPE,
                    cover_image_url = theProduct.COVER_PICTURE_PATH,
                    product_image_urls = thePictures,
                    product_details = theProduct.DETAILS
                };
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //取消发布商品的api
        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelProduct([FromBody] ProductIDViewModel model)
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

                var productID = model.ProductID;

                //删除MERCHANDISES表中的该商品的信息
                db.Deleteable<MERCHANDISES>()
                    .Where(d => d.MERCHANDISE_ID == productID)
                    .ExecuteCommand();
                //删除MERCHANDISES_PICTURE表中该商品的信息
                db.Deleteable<MERCHANDISES_PICTURE>()
                    .Where(t => t.MERCHANDISE_ID == productID)
                    .ExecuteCommand();
                //删除SELLS表中该商品的信息
                db.Deleteable<SELLS>()
                    .Where(s => s.MERCHANDISE_ID == productID)
                    .ExecuteCommand();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
