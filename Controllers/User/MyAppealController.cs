using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("myappeal")]
    [ApiController]
    public class MyAppealController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInfo()
        {
            SqlSugarClient db = Database.GetInstance();
            var role = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (role != "User")
            {
                return Unauthorized();
            }
            try
            {
                var appeals = db.Queryable<APPEALS, COMPLAINTS, BE_COMPLAINTED, COMMIT_COMPLAINT, USERS, USERS>(
                        (a, c, bc, cc, seller, buyer) => new object[] {
                            JoinType.Inner, a.COMPLAINT_ID == c.COMPLAINT_ID,
                            JoinType.Inner, a.COMPLAINT_ID == bc.COMPLAINT_ID,   // APPEALS 与 BE_COMPLAINTED 表连接
                            JoinType.Inner, a.COMPLAINT_ID == cc.COMPLAINT_ID,    // APPEALS 与 COMMIT_COMPLAINT 表连接
                            JoinType.Inner, bc.SELLER_ID == seller.USER_ID,       // BE_COMPLAINTED 与 USERS 表连接
                            JoinType.Inner, cc.CUSTOMER_ID == buyer.USER_ID   // COMMIT_COMPLAINT 与 USERS 表连接
                        })
                    .Where((a, c, bc, cc, seller, buyer) => seller.USER_ID == id)
                    .Select((a, c, bc, cc, seller, buyer) => new MyAppeal
                    {
                        user_name=buyer.NAME,
                        complation_reason=c.COMPLAINT_REASON,
                        appeal_time= a.APPEAL_TIME,
                        appeal_reason=a.APPEAL_REASON,
                        appeal_state=a.APPEAL_STATE,
                    })
                    .ToList();
                return Ok(appeals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
