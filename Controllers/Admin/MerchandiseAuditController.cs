using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UniTrade.Controllers.Admin
{
    [Route("merchandiseaudit")]
    [ApiController]
    public class MerchandiseAuditController : ControllerBase
    {
        [HttpGet("query")]
        public IActionResult GetInfo()
        {
            SqlSugarClient db = Database.GetInstance();
            try
            {
                //先查出所有的评价
                var comments = db.Queryable<COMMENT_ON, SCORES>(
                    (c, s) => new object[] {
                        JoinType.Inner,c.COMMENT_ID==s.COMMENT_ID
                    })
                    .Select((c, s) => new MerchandiseCommit
                    {
                        merchandise_id = c.MERCHANDISE_ID,
                        point = (s.QUALITY ? 1 : 0) + (s.ATTITUDE ? 1 : 0) + (s.PRICE ? 1 : 0) + (s.LOGISTIC_SPEED ? 1 : 0) + (s.CONFORMITY ? 1 : 0)
                    })
                    .ToList();
                //对评价进行分组，并计算平均值和总数
                var merchandises = comments.GroupBy(item => item.merchandise_id)
                    .Select(group => new MerchandiseCommitStatistics
                    {
                        merchandise_id = group.Key,
                        sum = group.Count(),
                        average = group.Average(item => item.point)
                    })
                    .OrderBy(item => item.average)
                    .Take(10)
                    .ToList();
                List<MerchandiseAuditInfo> merchandisesinfo = new List<MerchandiseAuditInfo>();
                //依据分组好的内容进表中查询其他数据
                foreach (var item in merchandises)
                {
                    var merchandiseinfo = db.Queryable<MERCHANDISES, SELLS, USERS>(
                        (m, s, u) => new object[]
                        {
                            JoinType.Inner,m.MERCHANDISE_ID==s.MERCHANDISE_ID,
                            JoinType.Inner,s.SELLER_ID==u.USER_ID,
                        })
                        .Where(m => m.MERCHANDISE_ID == item.merchandise_id)
                        .Select((m, s, u) => new MerchandiseAuditInfo
                        {
                            merchandise_id = item.merchandise_id,
                            merchandise_name = m.MERCHANDISE_NAME,
                            merchandise_type = m.MERCHANDISE_TYPE,
                            merchandise_price = m.PRICE,
                            seller_name = u.NAME,
                            reputation = u.REPUTATION,
                            commit_sum = item.sum,
                            average_point = item.average
                        })
                        .First();
                    if (merchandiseinfo != null)
                    {
                        merchandisesinfo.Add(merchandiseinfo);
                    }
                }
                return Ok(merchandisesinfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("offshelf")]
        public IActionResult AuditRefunds([FromBody] PullMerchandise result)
        {
            string merchandise_id = result.merchandise_id;
            SqlSugarClient db = Database.GetInstance();
            try
            {
                db.Deleteable<MERCHANDISES>()
                     .Where(m => m.MERCHANDISE_ID == merchandise_id)
                     .ExecuteCommand();
                /*db.Deleteable<SELLS>()
                     .Where(s => s.MERCHANDISE_ID == merchandise_id)
                     .ExecuteCommand();*/
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
