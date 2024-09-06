using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using UniTrade.Tools;
using UniTrade.Models;
using SqlSugar;
using UniTrade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UniTrade.Controllers.Cart
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly SqlSugarClient _db;

        public CartController()
        {
            _db = Database.GetInstance(); // 获取数据库实例
        }

        // 获取购物车列表
        [HttpGet]
        public ActionResult<IEnumerable<CartItemViewModel>> GetCartItems()
        {
            var customer_id = HttpContext.User.FindFirstValue(ClaimTypes.Name); // 获取用户id

            try
            {
                var items = _db.Queryable<CARTS, MERCHANDISES>((cart, merch) => new object[] {
                                    JoinType.Inner, cart.MERCHANDISE_ID == merch.MERCHANDISE_ID
                                })
                               .Where((cart, merch) => cart.CUSTOMER_ID == customer_id)
                               .Select((cart, merch) => new CartItemViewModel
                               {
                                   merchandise_id = cart.MERCHANDISE_ID,
                                   merchandise_name = merch.MERCHANDISE_NAME,
                                   merchandise_price = (double)merch.PRICE,
                                   picture = merch.COVER_PICTURE_PATH,
                                   quanity = (int)cart.QUANITY,
                                   cart_time = cart.CART_TIME,
                                   selected = true // 假设所有商品默认被选中，可以根据实际业务逻辑进行调整
                               }).ToList();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        // 添加商品到购物车
        [HttpPost]
        public ActionResult AddToCart([FromBody] CartItemViewModel cart_item)
        {
            var customer_id = HttpContext.User.FindFirstValue(ClaimTypes.Name); // 获取用户id

            try
            {
                // 检查购物车中是否已经存在相同的商品
                var existingItem = _db.Queryable<CARTS>()
                                      .Where(c => c.CUSTOMER_ID == customer_id && c.MERCHANDISE_ID == cart_item.merchandise_id)
                                      .First();

                if (existingItem != null)
                {
                    // 如果商品已存在，则更新数量
                    existingItem.QUANITY += cart_item.quanity; // 例如，增加商品数量
                    _db.Updateable(existingItem)
                        .Where(it => it.CUSTOMER_ID == customer_id && it.MERCHANDISE_ID == cart_item.merchandise_id) // 添加更新条件
                        .ExecuteCommand();
                    return Ok("购物车已更新。");
                }
                else
                {
                    // 如果商品不存在，则插入新记录
                    var newItem = new CARTS
                    {
                        CUSTOMER_ID = customer_id,
                        MERCHANDISE_ID = cart_item.merchandise_id,
                        QUANITY = cart_item.quanity,
                        CART_TIME = DateTime.Now // 假设添加到购物车时记录当前时间
                    };

                    _db.Insertable(newItem).ExecuteCommand();
                    return Ok("已成功添加到购物车。");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        // 从购物车删除商品
        [HttpDelete("{merchandise_id}")]
        public ActionResult RemoveFromCart(string merchandise_id)
        {
            try
            {
                var customer_id = HttpContext.User.FindFirstValue(ClaimTypes.Name); // 获取用户id

                var item = _db.Queryable<CARTS>()
                              .Where(c => c.CUSTOMER_ID == customer_id && c.MERCHANDISE_ID == merchandise_id)
                              .First();

                if (item != null)
                {
                    _db.Deleteable<CARTS>()
                       .Where(c => c.CUSTOMER_ID == customer_id && c.MERCHANDISE_ID == merchandise_id)
                       .ExecuteCommand();

                    return Ok("成功从购物车中删除。");
                }
                return BadRequest("购物车中没有该商品。");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        // 修改商品状态
        [HttpPost("update")]
        public ActionResult UpdateCartItem([FromBody] CartItemViewModel cart_item_update)
        {
            var customer_id = HttpContext.User.FindFirstValue(ClaimTypes.Name); // 获取用户id
            try
            {
                // 使用组合主键条件查询
                var item = _db.Queryable<CARTS>()
                              .Where(c => c.CUSTOMER_ID == customer_id && c.MERCHANDISE_ID == cart_item_update.merchandise_id)
                              .First();

                if (item != null)
                {
                    // 更新商品数量
                    item.QUANITY = cart_item_update.quanity;
                    // 更新其他可能的字段（如选中状态等）

                    // 使用组合主键条件更新
                    _db.Updateable(item)
                       .Where(c => c.CUSTOMER_ID == item.CUSTOMER_ID && c.MERCHANDISE_ID == item.MERCHANDISE_ID)
                       .ExecuteCommand();

                    return Ok("购物车项目更新成功。");
                }

                return NotFound("未找到购物车物品。");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }
    }
}
