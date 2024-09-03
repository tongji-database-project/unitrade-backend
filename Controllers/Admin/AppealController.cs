using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using UniTrade.Models;
using UniTrade.ViewModels;
using UniTrade.Tools;

namespace UniTrade.Controllers
{
    [Route("appeal")]
    [ApiController]
    public class AppealController : ControllerBase
    {
        // ��ȡ����������Ϣ
        [HttpGet("query")]
        public IActionResult GetAppeals()
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var appeals = db.Queryable<APPEALS, BE_COMPLAINTED, COMMIT_COMPLAINT, USERS, USERS>(
                        (a, b, c, seller, buyer) => new object[] {
                            JoinType.Inner, a.COMPLAINT_ID == b.COMPLAINT_ID,   // APPEALS �� BE_COMPLAINTED ������
                            JoinType.Inner, a.COMPLAINT_ID == c.COMPLAINT_ID,    // APPEALS �� COMMIT_COMPLAINT ������
                            JoinType.Inner, b.SELLER_ID == seller.USER_ID,       // BE_COMPLAINTED �� USERS ������
                            JoinType.Inner, c.CUSTOMER_ID == buyer.USER_ID   // COMMIT_COMPLAINT �� USERS ������
                        })
                        .Where(a => a.APPEAL_STATE == "Pen") // ����ȡ״̬Ϊ "Pending" �����߼�¼
                        .Select((a, b, c, seller, buyer) => new QueryAppealInfo
                        {
                            appeal_id = a.APPEAL_ID,
                            seller_id = b.SELLER_ID,
                            seller_name = seller.NAME,
                            complainant_id = c.CUSTOMER_ID,
                            complainant_name = buyer.NAME,
                            credibility = seller.REPUTATION,
                            reason = a.APPEAL_REASON,
                            time = a.APPEAL_TIME,
                        })
                        .ToList();
                return Ok(appeals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // ��������
        [HttpPost("audit")]
        public IActionResult AuditAppeals([FromBody] AuditAppealInfo result)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var appeal = db.Queryable<APPEALS>()
                .Where(a => a.APPEAL_ID == result.appeal_id && a.APPEAL_STATE == "Pen")
                .First();

                if (appeal == null)
                {
                    return NotFound("Appeal not found or already completed.");
                }
                else
                {
                    if (result.is_agreed == true)
                    {
                        appeal.APPEAL_STATE = "Agr";
                        var seller = db.Queryable<APPEALS, BE_COMPLAINTED, USERS>(
                            (a, b, u) => new object[] {
                                JoinType.Inner, a.COMPLAINT_ID==b.COMPLAINT_ID,
                                JoinType.Inner, b.SELLER_ID==u.USER_ID,
                        })
                       .Where(a => a.APPEAL_ID == result.appeal_id)
                       .Select((a, b, u) => u)
                       .First();

                        if (seller != null)
                        {
                            seller.REPUTATION += 5;
                            db.Updateable(seller).ExecuteCommand();
                        }
                    }
                    else
                    {
                        appeal.APPEAL_STATE = "Dis";
                    }
                    db.Updateable(appeal).ExecuteCommand();
                    return Ok("Appeal completed successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
