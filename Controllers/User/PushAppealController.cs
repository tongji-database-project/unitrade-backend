using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("pushappeal")]
    [ApiController]
    public class PushAppealController : ControllerBase
    {
        [HttpPost]
        public IActionResult AppealPush([FromBody] PushAppeal appealinfo)
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                int sum = db.Queryable<APPEALS>().Count() + 1;
                string id = sum.ToString("D20");
                APPEALS newappeal = new APPEALS
                {
                    APPEAL_ID = id,
                    APPEAL_STATE = "Pen",
                    APPEAL_REASON = appealinfo.appeal_reason,
                    APPEAL_FEEDBACK = "",
                    APPEAL_TIME = DateTime.Now,
                    COMPLAINT_ID = appealinfo.complation_id,
                };
                db.Insertable(newappeal).ExecuteCommand();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
