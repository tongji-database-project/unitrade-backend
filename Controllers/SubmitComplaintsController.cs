using Microsoft.AspNetCore.Mvc;
using UniTrade.Models;
using SqlSugar;
using System;
using System.Security.Claims;
using UniTrade.Tools;

namespace UniTrade.Controllers
{
    [Route("SubmitComplaintsController")]
    [ApiController]
    public class SubmitComplaintsController : ControllerBase
    {
        // 提交投诉
        [HttpPost("submit")]
        public IActionResult SubmitComplaint([FromBody] ComplaintRequest request)
        {
            SqlSugarClient db = Database.GetInstance();

            try
            {
                // 获取当前用户 ID
                var customerId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

                // 验证用户是否存在
                var customer = db.Queryable<USERS>()
                    .Where(u => u.USER_ID == customerId)
                    .First();

                if (customer == null)
                {
                    return Unauthorized("用户不存在");
                }

                // 创建新的投诉记录
                COMPLAINTS complaint = new COMPLAINTS
                {
                    COMPLAINT_ID = Guid.NewGuid().ToString(),
                    COMPLAINT_STATE = "PND",  // 初始状态为待处理
                    COMPLAINT_REASON = request.ComplaintReason,
                    COMPLAINT_TIME = DateTime.Now,
                };

                // 保存投诉记录到数据库
                db.Insertable(complaint).ExecuteCommand();

                // 创建买家与投诉的关系记录
                COMMIT_COMPLAINT commitComplaint = new COMMIT_COMPLAINT
                {
                    CUSTOMER_ID = customerId,  // 当前用户 ID
                    COMPLAINT_ID = complaint.COMPLAINT_ID
                };

                // 保存买家与投诉的关系到数据库
                db.Insertable(commitComplaint).ExecuteCommand();

                // 创建卖家与投诉的关系记录
                BE_COMPLAINTED beComplainted = new BE_COMPLAINTED
                {
                    SELLER_ID = request.SellerId,  // 从请求中获取的卖家 ID
                    COMPLAINT_ID = complaint.COMPLAINT_ID
                };

                // 保存卖家与投诉的关系到数据库
                db.Insertable(beComplainted).ExecuteCommand();

                return Ok("投诉提交成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"提交投诉时发生内部错误: {ex.Message}");
            }
        }
    }

    // 请求模型
    public class ComplaintRequest
    {
        public string SellerId { get; set; }  // 卖家 ID
        public string ComplaintReason { get; set; }
    }
}
