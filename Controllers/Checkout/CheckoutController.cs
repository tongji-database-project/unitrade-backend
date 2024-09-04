using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using UniTrade.Models;
using UniTrade.ViewModels;
using UniTrade.Tools;
using System.Threading.Tasks;
using SqlSugar;

namespace UniTrade.Controllers.Checkout
{
    [Route("[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly SqlSugarClient _db;

        public CheckoutController()
        {
            _db = Database.GetInstance(); // 使用 SqlSugar 获取数据库实例
        }

        /// <summary>
        /// 获取订单结算信息，包括用户信息和购物车选中商品信息。
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ActionResult<OrderSummaryViewModel>> GetCheckoutSummary(string userId)
        {
            var orderSummary = new OrderSummaryViewModel();

            try
            {
                // 获取用户信息
                var user = await _db.Queryable<USERS>()
                                   .Where(u => u.USER_ID == userId)
                                   .FirstAsync();
                if (user == null)
                {
                    return NotFound("用户不存在");
                }

                orderSummary.UserName = user.NAME;
                orderSummary.Phone = user.PHONE;
                orderSummary.Address = user.ADDRESS;

                // 获取购物车选中商品信息
                orderSummary.CartItems = await _db.Queryable<CARTS, MERCHANDISES>((c, m) => c.MERCHANDISE_ID == m.MERCHANDISE_ID)
                                                 .Where((c, m) => c.CUSTOMER_ID == userId)
                                                 .Select((c, m) => new MerchandiseViewModel
                                                 {
                                                     MerchandiseId = c.MERCHANDISE_ID,
                                                     Name = m.MERCHANDISE_NAME,
                                                     Quantity = c.QUANITY,
                                                     Price = m.PRICE,
                                                     Subtotal = m.PRICE * c.QUANITY
                                                 })
                                                 .ToListAsync();

                if (orderSummary.CartItems.Count == 0)
                {
                    return BadRequest("购物车中没有选中的商品");
                }

                // 从前端接收 selected 状态
                var selectedItems = orderSummary.CartItems.Where(item => item.Quantity > 0).ToList();

                // 计算总价和运费
                orderSummary.TotalPrice = selectedItems.Sum(item => item.Subtotal);
                orderSummary.ShippingFee = 10.0m; // 示例固定运费
                orderSummary.GrandTotal = orderSummary.TotalPrice + orderSummary.ShippingFee;

                return Ok(orderSummary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 生成订单。
        /// </summary>
        [HttpPost("create-order")]
        public async Task<ActionResult> CreateOrder([FromBody] OrderSummaryViewModel orderSummary)
        {
            if (orderSummary == null || orderSummary.CartItems == null || !orderSummary.CartItems.Any())
            {
                return BadRequest("无效的订单数据");
            }

            // 使用 SqlSugar 事务
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                foreach (var item in orderSummary.CartItems.Where(x => x.Quantity > 0))
                {
                    var orderId = Guid.NewGuid().ToString();
                    var order = new ORDERS
                    {
                        ORDER_ID = orderId,
                        MERCHANDISE_ID = item.MerchandiseId,
                        STATE = "待发货", // 初始状态为待发货
                        ORDER_QUANITY = item.Quantity,
                        ORDER_TIME = DateTime.UtcNow
                    };

                    await _db.Insertable(order).ExecuteCommandAsync();
                }
            });

            if (result.IsSuccess)
            {
                return Ok("订单创建成功");
            }
            else
            {
                return StatusCode(500, $"服务器错误：{result.ErrorMessage}");
            }
        }
    }
}
