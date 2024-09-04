using Microsoft.AspNetCore.Mvc;
using UniTrade.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using UniTrade.Tools;
using System.Security.Claims;

namespace UniTrade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerComplaintsController : ControllerBase
    {
        // 提交投诉
        [HttpPost("submit")]
        public IActionResult SubmitComplaint([FromBody] ComplaintRequest request)
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

                // 创建新的投诉记录
                COMPLAINTS complaint = new COMPLAINTS
                {
                    COMPLAINT_ID = Guid.NewGuid().ToString(),
                    COMPLAINT_STATE = "PND",  // 初始状态为待处理
                    COMPLAINT_REASON = request.ComplaintReason,
                    COMPLAINT_TIME = DateTime.Now,
                };

                // 创建被投诉商家的关系记录
                BE_COMPLAINTED beComplainted = new BE_COMPLAINTED
                {
                    SELLER_ID = request.SellerId,
                    COMPLAINT_ID = complaint.COMPLAINT_ID
                };

                // 将数据保存到数据库
                db.Insertable(complaint).ExecuteCommand();
                db.Insertable(beComplainted).ExecuteCommand();

                return Ok("投诉提交成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"提交投诉时发生内部错误: {ex.Message}");
            }
        }

        // 获取卖家的所有投诉
        [HttpGet("{sellerId}/complaints")]
        public IActionResult GetComplaintsBySeller(string sellerId)
        {
            SqlSugarClient db = Database.GetInstance();

            try
            {
                // 查询指定卖家的所有投诉
                var complaints = db.Queryable<COMPLAINTS, BE_COMPLAINTED>((c, bc) => new object[]
                    {
                        JoinType.Inner, c.COMPLAINT_ID == bc.COMPLAINT_ID
                    })
                    .Where((c, bc) => bc.SELLER_ID == sellerId)
                    .Select((c, bc) => new
                    {
                        c.COMPLAINT_ID,
                        c.COMPLAINT_STATE,
                        c.COMPLAINT_REASON,
                        c.COMPLAINT_FEEDBACK,
                        c.COMPLAINT_TIME
                    })
                    .ToList();

                if (complaints.Count == 0)
                {
                    return NotFound("没有找到相关的投诉记录");
                }

                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"查询投诉时发生内部错误: {ex.Message}");
            }
        }

        // 更新投诉状态（仅管理员使用）
        [HttpPut("update/{complaintId}")]
        public IActionResult UpdateComplaintStatus(string complaintId, [FromBody] ComplaintUpdateRequest request)
        {
            SqlSugarClient db = Database.GetInstance();

            try
            {
                // 查找要更新的投诉记录
                var complaint = db.Queryable<COMPLAINTS>()
                    .Where(c => c.COMPLAINT_ID == complaintId)
                    .First();

                if (complaint == null)
                {
                    return NotFound("未找到该投诉记录");
                }

                // 更新投诉状态和反馈
                complaint.COMPLAINT_STATE = request.ComplaintState;
                complaint.COMPLAINT_FEEDBACK = request.ComplaintFeedback;

                // 保存更改
                db.Updateable(complaint).ExecuteCommand();

                return Ok("投诉状态更新成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新投诉状态时发生内部错误: {ex.Message}");
            }
        }
    }

    // 请求模型
    public class ComplaintRequest
    {
        public string SellerId { get; set; }
        public string ComplaintReason { get; set; }
    }

    public class ComplaintUpdateRequest
    {
        public string ComplaintState { get; set; }
        public string ComplaintFeedback { get; set; }
    }
}
