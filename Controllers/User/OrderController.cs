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
        [HttpPost("addComment")]
        public async Task<ActionResult<List<COMMENTS>>> AddComment([FromForm] AddCommentViewModel model)
        {
            try
            {
                Console.WriteLine($"Received OrderId: {model.OrderId}");
                Console.WriteLine($"Received MerchandiseId: {model.MerchandiseId}");
                Console.WriteLine($"Received Content: {model.Content}");
                Console.WriteLine($"Received CommentType: {model.CommentType}");
                using (var db = Database.GetInstance())
                {
                    // 处理文件上传
                    byte[] fileBytes = null;
                    if (model.CommentPicture != null && model.CommentPicture.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.CommentPicture.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }
                    }

                    // 创建评论对象
                    var comment = new COMMENTS
                    {
                        COMMENT_ID = Guid.NewGuid().ToString(),
                        CONTENT = model.Content,
                        COMMENT_TIME = DateTime.Now,
                        COMMENT_TYPE = model.CommentType,
                        COMMENT_PICTURE = fileBytes
                    };

                    // 插入评论
                    await db.Insertable(comment).ExecuteCommandAsync();

                    // 创建评论与订单商品的关系
                    var commentOn = new COMMENT_ON
                    {
                        COMMENT_ID = comment.COMMENT_ID,
                        ORDER_ID = model.OrderId,
                        MERCHANDISE_ID = model.MerchandiseId
                    };

                    // 插入评论与订单商品的关系
                    await db.Insertable(commentOn).ExecuteCommandAsync();

                    return Ok("评论已成功添加！");
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
        public async Task<IActionResult> ConfirmReceipt([FromBody] ConfirmReceiptViewModel model)
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

                    // 检查订单和商品是否存在
                    var orderExists = await db.Queryable<ORDERS>()
                        .AnyAsync(o => o.ORDER_ID == model.OrderId && o.MERCHANDISE_ID == model.MerchandiseId);

                    if (!orderExists)
                    {
                        return BadRequest("订单或商品信息无效");
                    }

                    // 插入 HOLDS 表记录
                    var holds = new HOLDS
                    {
                        SELLER_ID = (await db.Queryable<SELLS>()
                            .Where(s => s.MERCHANDISE_ID == model.MerchandiseId)
                            .Select(s => s.SELLER_ID)
                            .FirstAsync()) ?? throw new InvalidOperationException("未找到卖家"),
                        MERCHANDISE_ID = model.MerchandiseId,
                        ORDER_ID = model.OrderId
                    };
                    await db.Insertable(holds).ExecuteCommandAsync();

                    // 更新订单状态
                    var order = await db.Queryable<ORDERS>()
                        .Where(o => o.ORDER_ID == model.OrderId)
                        .FirstAsync();

                    if (order == null)
                    {
                        return NotFound("未找到订单");
                    }

                    order.STATE = "ysh"; // 更新状态为已收货
                    await db.Updateable(order).ExecuteCommandAsync();

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
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestViewModel model)
        {
            try
            {
                using (var db = Database.GetInstance())
                {
                    // 确保订单存在
                    var exists = db.Queryable<ORDERS>()
                        .Where(o => o.ORDER_ID == model.OrderId)
                        .Any();

                    if (!exists)
                    {
                        return NotFound("订单未找到");
                    }

                    // 查找订单
                    var order = db.Queryable<ORDERS>()
                        .Where(o => o.ORDER_ID == model.OrderId)
                        .Take(1) // 确保只取一个记录
                        .ToList() // 转换为列表
                        .First(); // 取第一个记录

                    // 更新订单状态
                    order.STATE = "yjs";
                    await db.Updateable(order).ExecuteCommandAsync();

                    // 创建退款记录
                    var refund = new REFUNDS
                    {
                        REFUND_ID = Guid.NewGuid().ToString(),
                        REFUND_STATE = "Pending",
                        REFUND_REASON = model.RefundReason,
                        REFUND_FEEDBACK = model.RefundFeedback,
                        REFUND_TIME = DateTime.Now
                    };
                    await db.Insertable(refund).ExecuteCommandAsync();

                    // 创建退款与订单商品的关系
                    var refundOn = new REFUND_ON
                    {
                        REFUND_ID = refund.REFUND_ID,
                        ORDER_ID = model.OrderId,
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
