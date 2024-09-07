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

        [Authorize]
        [HttpGet("user/address")]
        public async Task<ActionResult<string>> GetUserAddress()
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

                    // 查询用户地址
                    var userAddress = await db.Queryable<USERS>()
                        .Where(u => u.USER_ID == userId)
                        .Select(u => u.ADDRESS)
                        .FirstAsync(); // 获取用户的地址

                    if (string.IsNullOrEmpty(userAddress))
                    {
                        return NotFound("未找到用户地址信息");
                    }

                    return Ok(userAddress);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }

        [Authorize]
        [HttpGet("merchandise/name")]
        public async Task<ActionResult<string>> GetMerchandiseName(string merchandise_id)
        {
            try
            {
                using (var db = Database.GetInstance()) // 使用 using 语句简化数据库连接管理
                {
                    // 查询商品名称
                    var merchandiseName = await db.Queryable<MERCHANDISES>()
                        .Where(m => m.MERCHANDISE_ID == merchandise_id)
                        .Select(m => m.MERCHANDISE_NAME)
                        .FirstAsync(); // 获取商品名称
                    Console.WriteLine(merchandiseName);
                    if (string.IsNullOrEmpty(merchandiseName))
                    {
                        return NotFound("未找到商品信息");
                    }

                    return Ok(merchandiseName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }

        [Authorize]
        [HttpPost("addComment")]
        public async Task<ActionResult> AddComment(string order_id,string merchandise_id,string content,string comment_type,short quality_rating,short attitude_rating,short price_rating,short logistic_speed_rating,short conformity_rating)
        {
            try
            {
                Console.WriteLine($"Received order_id: {order_id}");
                Console.WriteLine($"Received merchandise_id: {merchandise_id}");
                Console.WriteLine($"Received content: {content}");
                Console.WriteLine($"Received comment_type: {comment_type}");

                using (var db = Database.GetInstance())
                {
                    // 创建评论对象并插入到 COMMENTS 表
                    var comment = new COMMENTS
                    {
                        COMMENT_ID = Guid.NewGuid().ToString(),  // 生成新的评论ID
                        CONTENT = content,  // 评论内容（前端传入）
                        COMMENT_TIME = DateTime.Now,  // 当前时间作为评论时间
                        COMMENT_TYPE = comment_type,  // 评论类型（前端传入）
                    };

                    // 插入评论到 COMMENTS 表
                    await db.Insertable(comment).ExecuteCommandAsync();

                    // 创建 COMMENT_ON 对象并插入到 COMMENT_ON 表，建立评论与订单、商品的关系
                    var commentOn = new COMMENT_ON
                    {
                        COMMENT_ID = comment.COMMENT_ID,  // 评论ID
                        ORDER_ID = order_id,  // 订单ID（前端传入）
                        MERCHANDISE_ID = merchandise_id  // 商品ID（前端传入）
                    };

                    // 插入关系到 COMMENT_ON 表
                    await db.Insertable(commentOn).ExecuteCommandAsync();

                    // 插入评分到 SCORES 表
                    var scores = new SCORES
                    {
                        COMMENT_ID = comment.COMMENT_ID,  // 评论ID
                        QUALITY = quality_rating,  // 质量评分（前端传入）
                        ATTITUDE = attitude_rating,  // 态度评分（前端传入）
                        PRICE = price_rating,  // 价格评分（前端传入）
                        LOGISTIC_SPEED = logistic_speed_rating,  // 物流速度评分（前端传入）
                        CONFORMITY = conformity_rating  // 描述相符评分（前端传入）
                    };

                    // 插入评分到 SCORES 表
                    await db.Insertable(scores).ExecuteCommandAsync();

                    return Ok("评论和评分已成功添加！");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }

        [Authorize]
        [HttpPost("confirmReceipt")]
        //public async Task<IActionResult> ConfirmReceipt([FromBody] ConfirmReceiptViewModel model)
        public async Task<IActionResult> ConfirmReceipt(string order_id, string merchandise_id)
        {
            try
            {
                using (var db = Database.GetInstance())
                {
                    // 获取当前用户ID
                    var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized("用户未认证");
                    }
                    Console.WriteLine($"{nameof(ConfirmReceipt)}: {userId}");
                    // 检查订单和商品是否存在
                    var orderExists = await db.Queryable<ORDERS>()
                        .AnyAsync(o => o.ORDER_ID == order_id && o.MERCHANDISE_ID == merchandise_id);

                    if (!orderExists)
                    {
                        return BadRequest("订单或商品信息无效");
                    }
                    Console.WriteLine("订单找到了");
                    // 插入 HOLDS 表记录
                    var holds = new HOLDS
                    {
                        SELLER_ID = (await db.Queryable<SELLS>()
                            .Where(s => s.MERCHANDISE_ID == merchandise_id)
                            .Select(s => s.SELLER_ID)
                            .FirstAsync()) ?? throw new InvalidOperationException("未找到卖家"),
                        MERCHANDISE_ID = merchandise_id,
                        ORDER_ID = order_id
                    };
                    await db.Insertable(holds).ExecuteCommandAsync();
                    Console.WriteLine("卖家找到了");

                    // 更新订单状态
                    var updateResult = await db.Updateable<ORDERS>()
                        .SetColumns(o => o.STATE == "Rec")  // 设置更新状态
                        .Where(o => o.ORDER_ID == order_id)  // 更新条件
                        .ExecuteCommandAsync();

                    Console.WriteLine("更新完成");

                    if (updateResult == 0)
                    {
                        return NotFound("未找到订单");
                    }

                    Console.WriteLine("更新订单状态为已收货");
                    return Ok("收货确认成功");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "服务器内部错误");
            }
        }

        [Authorize]
        [HttpPost("requestRefund")]
        public async Task<IActionResult> RequestRefund(string order_id,string refund_reason,string refund_feedback)
        {
            try
            {
                using (var db = Database.GetInstance())
                {
                    // 确保订单存在
                    var exists = db.Queryable<ORDERS>()
                        .Where(o => o.ORDER_ID == order_id)
                        .Any();

                    if (!exists)
                    {
                        return NotFound("订单未找到");
                    }
                    Console.WriteLine("找到订单了");
                    // 查找订单
                    var order = db.Queryable<ORDERS>()
                        .Where(o => o.ORDER_ID == order_id)
                        .Take(1) // 确保只取一个记录
                        .ToList() // 转换为列表
                        .First(); // 取第一个记录
                    Console.WriteLine("找到记录了");
                    // 更新订单状态   
                    var updateResult = await db.Updateable<ORDERS>()
                        .SetColumns(o => o.STATE == "Pen")  // 设置更新状态
                        .Where(o => o.ORDER_ID == order_id)  // 更新条件
                        .ExecuteCommandAsync();

                    Console.WriteLine("更新完成");

                    // 创建退款记录
                    var refund = new REFUNDS
                    {
                        REFUND_ID = Guid.NewGuid().ToString(),
                        REFUND_STATE = "Pen",
                        REFUND_REASON = refund_reason,
                        REFUND_FEEDBACK = refund_feedback,
                        REFUND_TIME = DateTime.Now
                    };
                    await db.Insertable(refund).ExecuteCommandAsync();

                    // 创建退款与订单商品的关系
                    var refundOn = new REFUND_ON
                    {
                        REFUND_ID = refund.REFUND_ID,
                        ORDER_ID = order_id,
                        MERCHANDISE_ID = order.MERCHANDISE_ID
                    };
                    await db.Insertable(refundOn).ExecuteCommandAsync();

                    return Ok("退款申请已成功处理");
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
