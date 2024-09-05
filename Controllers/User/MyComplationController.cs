using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("mycomplation")]
    [ApiController]
    public class MyComplationController : ControllerBase
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
                var complations = db.Queryable<COMPLAINTS, COMMIT_COMPLAINT, BE_COMPLAINTED, USERS, USERS>(
                    (c, cc, bc, u1, u2) => new object[] {
                        JoinType.Inner,c.COMPLAINT_ID==cc.COMPLAINT_ID,
                        JoinType.Inner,c.COMPLAINT_ID==bc.COMPLAINT_ID,
                        JoinType.Inner,cc.CUSTOMER_ID==u1.USER_ID,
                        JoinType.Inner,bc.SELLER_ID==u2.USER_ID,
                    })
                    .Where((c, cc, bc, u1, u2) => u1.USER_ID==id)
                    .Select((c, cc, bc, u1, u2) => new MyComplation
                    {
                        seller_name = u2.NAME,
                        complation_reason=c.COMPLAINT_REASON,
                        complation_time= c.COMPLAINT_TIME,
                        complation_state=c.COMPLAINT_STATE,
                    })
                    .ToList();
                return Ok(complations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
