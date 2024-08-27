using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Admin
{
    [Route("complationAudit")]
    [ApiController]
    public class ComplationAuditController : ControllerBase
    {
        [HttpGet("getInfo")]
        public IActionResult GetInfo()
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                var complations = db.Queryable<COMPLAINTS, COMMIT_COMPLAINT, BE_COMPLAINTED, USERS, USERS>(
                    (c, cc, bc, u1, u2) => new object[] {
                        JoinType.Inner,c.COMPLAINT_ID==cc.COMPLAINT_ID,
                        JoinType.Inner,c.COMPLAINT_ID==bc.COMPLAINT_ID,
                        JoinType.Inner,cc.CUSTOMER_ID==u1.USER_ID,
                        JoinType.Inner,bc.SELLER_ID==u2.USER_ID,
                    })
                    .Where(c => c.COMPLAINT_STATE == "Pending")
                    .Select((c, cc, bc, u1, u2) => new ComplationAuditInfo
                    {
                        complation_id = c.COMPLAINT_ID,
                        seller_name = u2.NAME,
                        reputation = u2.REPUTATION,
                        compaltion_reason = c.COMPLAINT_REASON,
                        customer_name = u1.NAME,
                        complation_time = (DateTime)c.COMPLAINT_TIME,
                    })
                    .Take(10)
                    .ToList();
                return Ok(complations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("audit")]
        public IActionResult Audit([FromBody] ComplationAuditResult result)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                int num1 = db.Updateable<COMPLAINTS>()
                    .Where(c => c.COMPLAINT_ID == result.complation_id && c.COMPLAINT_STATE == "Pending")
                    .SetColumns(c => new COMPLAINTS { COMPLAINT_STATE = "Completed" })
                    .ExecuteCommand();

                if (num1 != 0 && result.is_passed)
                {
                    var seller = db.Queryable<COMPLAINTS, BE_COMPLAINTED, USERS>(
                        (c, bc, u) => new object[] {
                            JoinType.Inner,c.COMPLAINT_ID==bc.COMPLAINT_ID,
                            JoinType.Inner,bc.SELLER_ID==u.USER_ID,
                        })
                        .Where(c => c.COMPLAINT_ID == result.complation_id)
                        .Select((c, bc, u) => new { id = u.USER_ID, reputation = u.REPUTATION })
                        .First();

                    if (seller != null)
                    {
                        var num2 = db.Updateable<USERS>()
                            .Where(u => u.USER_ID == seller.id)
                            .SetColumns(u => new USERS { REPUTATION = (short)(seller.reputation - 5 > 0 ? seller.reputation - 5 : 0) })
                            .ExecuteCommand();
                        return Ok();
                    }
                    else
                    {
                        return Unauthorized("数据库数据缺失");
                    }
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
