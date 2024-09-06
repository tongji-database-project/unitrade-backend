using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [Authorize]
        [HttpPost] // 将 HttpGet 改为 HttpPost
        public async Task<ActionResult<List<ORDERS>>> GetOrderById([FromBody] OrderListViewModel model)
        {
            try
            {
                using (var db = Database.GetInstance()) // 使用 using 语句简化数据库连接管理
                {
                    // 获取当前用户ID
                    var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized("用户未认证");
                    }

                    var userOrders = await db.Queryable<PLACES>()
                        .Where(u => u.CUSTOMER_ID == userId)
                        .Select(u => u.ORDER_ID)
                        .ToListAsync();

                    if (userOrders == null || !userOrders.Any())
                    {
                        return NotFound("未找到相关订单");
                    }

                    var query = db.Queryable<ORDERS>().In(o => o.ORDER_ID, userOrders);

                    if (!string.IsNullOrEmpty(model.order_id))
                    {
                        query = query.Where(o => o.ORDER_ID == model.order_id);
                    }

                    if (!string.IsNullOrEmpty(model.state))
                    {
                        query = query.Where(o => o.STATE == model.state);
                    }

                    var orderInfo = await query.ToListAsync();

                    if (orderInfo == null || !orderInfo.Any())
                    {
                        return NotFound("未找到相关信息");
                    }

                    Console.WriteLine($"用户 {userId} 的订单信息已成功读取。订单数量: {orderInfo.Count}");
                    Console.WriteLine("返回的订单数据: " + Newtonsoft.Json.JsonConvert.SerializeObject(orderInfo));
                    return Ok(orderInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }
    }
}
