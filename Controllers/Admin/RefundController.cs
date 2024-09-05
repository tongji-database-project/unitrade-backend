using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SqlSugar;
using System;
using UniTrade.Models;
using UniTrade.ViewModels;
using UniTrade.Tools;

namespace UniTrade.Controllers
{
    [Route("refund")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        // 获取所有退款信息
        [HttpGet("query")]
        public IActionResult GetRefunds()
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var refunds = db.Queryable<REFUNDS, REFUND_ON, HOLDS, PLACES, MERCHANDISES, USERS, USERS>(
                        (r, ro, h, p, m, seller, buyer) => new object[] {
                            JoinType.Inner, r.REFUND_ID == ro.REFUND_ID,   // REFUNDS 与 REFUND_ON 表连接
                            JoinType.Inner, ro.ORDER_ID == h.ORDER_ID,    // REFUND_ON 与 HOLDS 表连接
                            JoinType.Inner, ro.ORDER_ID == p.ORDER_ID,    // REFUND_ON 与 PLACES 表连接
                            JoinType.Inner, ro.MERCHANDISE_ID == m.MERCHANDISE_ID,  //REFUND_ON 与 MERCHANDISE 表连接
                            JoinType.Inner, h.SELLER_ID == seller.USER_ID,   // HOLDS 与 USERS 表连接
                            JoinType.Inner, p.CUSTOMER_ID == buyer.USER_ID      // PLACES 与 USERS 表连接
                        })
                        .Where(r => r.REFUND_STATE == "Pen") // 仅获取状态为 "Pending" 的退款记录
                        .Select((r, ro, h, p, m, seller, buyer) => new QueryRefundInfo
                        {
                            refund_id = r.REFUND_ID,
                            seller_id = h.SELLER_ID,
                            seller_name = seller.NAME,
                            buyer_id = p.CUSTOMER_ID,
                            buyer_name = buyer.NAME,
                            merchandise_name = m.MERCHANDISE_NAME,
                            reason = r.REFUND_REASON,
                            time = r.REFUND_TIME,
                        })
                        .ToList();
                return Ok(refunds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 处理退款申请
        [HttpPost("audit")]
        public IActionResult AuditRefunds([FromBody] AuditRefundInfo result)
        {
            
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var refund = db.Queryable<REFUNDS>()
                .Where(r => r.REFUND_ID == result.refund_id && r.REFUND_STATE == "Pen")
                .First();
                if (refund == null)
                {
                    return NotFound("Refund not found or already completed.");
                }
                else
                {
                    db.Updateable<REFUNDS>()
                            .Where(r => r.REFUND_ID == result.refund_id)
                            .SetColumns(r => new REFUNDS { REFUND_STATE = (result.is_agreed?"Agr":"Dis") })
                            .ExecuteCommand();
                    return Ok("Refund completed successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}