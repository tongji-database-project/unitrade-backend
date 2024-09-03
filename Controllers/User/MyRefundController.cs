using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("myrefund")]
    [ApiController]
    public class MyRefundController : ControllerBase
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
                var refunds = db.Queryable<REFUNDS, REFUND_ON, HOLDS, PLACES, MERCHANDISES, USERS, USERS>(
                        (r, ro, h, p, m, seller, buyer) => new object[] {
                            JoinType.Inner, r.REFUND_ID == ro.REFUND_ID,   // REFUNDS 与 REFUND_ON 表连接
                            JoinType.Inner, ro.ORDER_ID == h.ORDER_ID,    // REFUND_ON 与 HOLDS 表连接
                            JoinType.Inner, ro.ORDER_ID == p.ORDER_ID,    // REFUND_ON 与 PLACES 表连接
                            JoinType.Inner, ro.MERCHANDISE_ID == m.MERCHANDISE_ID,  //REFUND_ON 与 MERCHANDISE 表连接
                            JoinType.Inner, h.SELLER_ID == seller.USER_ID,   // HOLDS 与 USERS 表连接
                            JoinType.Inner, p.CUSTOMER_ID == buyer.USER_ID      // PLACES 与 USERS 表连接
                        })
                    .Where((r, ro, h, p, m, seller, buyer) => buyer.USER_ID == id)
                    .Select((r, ro, h, p, m, seller, buyer) => new MyRefund
                    {
                        order_id = ro.ORDER_ID,
                        merchandise_name = m.MERCHANDISE_NAME,
                        refund_reason = r.REFUND_REASON,
                        refund_time = ((DateTime)r.REFUND_TIME).ToString("o"),
                        refund_state = r.REFUND_STATE
                    })
                    .ToList();
                return Ok(refunds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
