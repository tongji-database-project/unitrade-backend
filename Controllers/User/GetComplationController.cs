using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("getcomplation")]
    [ApiController]
    public class GetComplationController : ControllerBase
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
                    .Where((c, cc, bc, u1, u2) => u2.USER_ID == id && c.COMPLAINT_STATE == "Agr")
                    .Select((c, cc, bc, u1, u2) => new HaveComplained
                    {
                        complation_id = c.COMPLAINT_ID,
                        user_name = u1.NAME,
                        complation_reason = c.COMPLAINT_REASON,
                        complation_time = c.COMPLAINT_TIME,
                        have_appealed=false
                    })
                    .ToList();
                foreach(HaveComplained item in complations)
                {
                    var num = db.Queryable<COMPLAINTS, APPEALS>(
                        (c, a) => new object[]
                        {
                            JoinType.Inner,c.COMPLAINT_ID==a.COMPLAINT_ID,
                        })
                        .Where(c => c.COMPLAINT_ID == item.complation_id)
                        .Count();
                    if (num > 0)
                    {
                        item.have_appealed = true;
                    }
                }
                return Ok(complations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
