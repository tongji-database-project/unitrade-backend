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
        // ��ȡ�����˿���Ϣ
        [HttpGet("query")]
        public IActionResult GetRefunds()
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var refunds = db.Queryable<REFUNDS, REFUND_ON, HOLDS, PLACES, MERCHANDISES, USERS, USERS>(
                        (r, ro, h, p, m, seller, buyer) => new object[] {
                            JoinType.Inner, r.REFUND_ID == ro.REFUND_ID,   // REFUNDS �� REFUND_ON ������
                            JoinType.Inner, ro.ORDER_ID == h.ORDER_ID,    // REFUND_ON �� HOLDS ������
                            JoinType.Inner, ro.ORDER_ID == p.ORDER_ID,    // REFUND_ON �� PLACES ������
                            JoinType.Inner, ro.MERCHANDISE_ID == m.MERCHANDISE_ID,  //REFUND_ON �� MERCHANDISE ������
                            JoinType.Inner, h.SELLER_ID == seller.USER_ID,   // HOLDS �� USERS ������
                            JoinType.Inner, p.CUSTOMER_ID == buyer.USER_ID      // PLACES �� USERS ������
                        })
                        .Where(r => r.REFUND_STATE == "Pen") // ����ȡ״̬Ϊ "Pending" ���˿��¼
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

        // �����˿�����
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