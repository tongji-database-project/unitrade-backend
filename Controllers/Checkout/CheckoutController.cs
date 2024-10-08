﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using UniTrade.Models;
using UniTrade.ViewModels;
using UniTrade.Tools;
using System.Threading.Tasks;
using SqlSugar;
using System.Security.Claims;
using Aop.Api.Domain;

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
        [HttpGet]
        public async Task<ActionResult<OrderSummaryViewModel>> GetCheckoutSummary()
        {
            var order_summary = new OrderSummaryViewModel()
            {
                cart_items = new List<CartItemViewModel>() // 确保CartItems初始化为非空列表
            };

            var user_id = HttpContext.User.FindFirstValue(ClaimTypes.Name); // 获取用户id

            try
            {
                // Console.WriteLine($"开始获取用户信息，用户ID: {user_id}");
                // 获取用户信息
                var user = await _db.Queryable<USERS>()
                                   .Where(u => u.USER_ID == user_id)
                                   .FirstAsync();

                if (user == null)
                {
                    return NotFound("用户不存在");
                }

                order_summary.user_name = user.NAME;
                order_summary.phone = user.PHONE;
                order_summary.address = user.ADDRESS;

                // Console.WriteLine("开始获取购物车选中商品信息...");
                // 获取购物车选中商品信息
                order_summary.cart_items = await _db.Queryable<CARTS, MERCHANDISES>((c, m) => c.MERCHANDISE_ID == m.MERCHANDISE_ID)
                                                 .Where((c, m) => c.CUSTOMER_ID == user_id && c.SELECTED == true)
                                                 .Select((c, m) => new CartItemViewModel
                                                 {
                                                     merchandise_id = c.MERCHANDISE_ID,
                                                     merchandise_name = m.MERCHANDISE_NAME,
                                                     merchandise_price = (double)m.PRICE,
                                                     picture = m.COVER_PICTURE_PATH,
                                                     quanity = (int)c.QUANITY,
                                                     cart_time = c.CART_TIME,
                                                     selected = c.SELECTED
                                                 })
                                                 .ToListAsync();

                if (order_summary.cart_items.Count == 0)
                {
                    Console.WriteLine("购物车为空或未找到符合条件的商品。");
                    return BadRequest("购物车中没有选中的商品");
                }

                // 计算总价和运费
                order_summary.total_price = order_summary.cart_items
                                            .Where(item => item.selected)
                                            .Sum(item => (decimal)item.merchandise_price * item.quanity);
                order_summary.shipping_fee = 10.0m; // 示例固定运费
                order_summary.grand_total = order_summary.total_price + order_summary.shipping_fee;

                // Console.WriteLine($"总价计算完成，总价: {order_summary.total_price}, 运费: {order_summary.shipping_fee}");

                return Ok(order_summary);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"服务器错误：{ex.Message}");
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }


        /// <summary>
        /// 生成订单。
        /// </summary>
        [HttpPost("create-order")]
        public async Task<ActionResult> CreateOrder([FromBody] OrderSummaryViewModel order_summary)
        {
            if (order_summary == null || order_summary.cart_items == null || !order_summary.cart_items.Any())
            {
                return BadRequest("无效的订单数据");
            }

            List<string> orderIds = new List<string>();
            var user_id = HttpContext.User.FindFirstValue(ClaimTypes.Name); // 获取用户id

            // 使用 SqlSugar 事务
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var orderId = Guid.NewGuid().ToString(); // 生成随机的订单id
                foreach (var item in order_summary.cart_items.Where(x => x.selected && x.quanity > 0))
                {
                    var order = new ORDERS
                    {
                        ORDER_ID = orderId,
                        MERCHANDISE_ID = item.merchandise_id,
                        STATE = "UNP", // 初始状态为未支付
                        ORDER_QUANITY = item.quanity,
                        ORDER_TIME = DateTime.Now
                    };

                    await _db.Insertable(order).ExecuteCommandAsync();

                    // 检查是否存在相同的 USER_ID, MERCHANDISE_ID, 和 ORDER_ID 组合
                    var existingPlace = await _db.Queryable<PLACES>()
                        .Where(p => p.CUSTOMER_ID == user_id && p.MERCHANDISE_ID == item.merchandise_id && p.ORDER_ID == orderId)
                        .FirstAsync();

                    // 如果记录不存在，则插入
                    if (existingPlace == null)
                    {
                        var place = new PLACES
                        {
                            CUSTOMER_ID = user_id,
                            MERCHANDISE_ID = item.merchandise_id,
                            ORDER_ID = orderId
                        };

                        await _db.Insertable(place).ExecuteCommandAsync();
                    }

                    orderIds.Add(orderId);
                }
            });

            if (result.IsSuccess)
            {
                return Ok(orderIds);
            }
            else
            {
                return StatusCode(500, $"服务器错误：{result.ErrorMessage}");
            }
        }
    }
}
