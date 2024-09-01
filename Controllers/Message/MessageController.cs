using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniTrade.Tools;
using UniTrade.Models;
using SqlSugar;

namespace UniTrade.Controllers.Message
{
    [Route("message")]
    [Authorize]
    [ApiController]
    public class MessageController : Controller
    {
        // GET: /message/get_contacting_user
        [HttpGet]
        [Route("get_contacting_user")]
        public async Task<ActionResult<IEnumerable<string>>> GetContactingUser()
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            try {
                var results = await db
                    .Queryable<COMMUNICATION>()
                    .Where(it => it.SENDER_ID == userIdClaim || it.RECEIVER_ID == userIdClaim)
                    .Select(it => it.RECEIVER_ID == userIdClaim ? it.SENDER_ID : it.RECEIVER_ID)
                    .ToListAsync();


                if (results != null)
                    return Ok(results);
                else
                    return NotFound("未找到相关信息");
            }
            catch (Exception ex) {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        // GET: /message/get_latest_message?user_id=1238
        // user_id 为另一方的 ID
        [HttpGet]
        [Route("get_latest_message")]
        public async Task<ActionResult<string>> GetLatestMessage(string user_id)
        {
            // TODO: 增加 ViewModel 以提供用户名与最新消息
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            try {
                var result = await db
                    .Queryable<COMMUNICATION>()
                    .Where(it =>
                            it.SENDER_ID == userIdClaim && it.RECEIVER_ID == user_id
                            || it.SENDER_ID == user_id && it.RECEIVER_ID == userIdClaim)
                    .OrderBy(it => SqlFunc.Desc(it.COMMUNICATION_TIME))
                    .Select(it => it.COMMUNICATION_CONTENT)
                    .FirstAsync();

                if (result != null)
                    return Ok(result);
                else
                    return NotFound("未找到相关信息");
            }
            catch (Exception ex) {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        // GET: /message/get_messages?user_id=2598
        // user_id 为另一方的 ID
        [HttpGet]
        [Route("get_messages")]
        public async Task<ActionResult<IEnumerable<COMMUNICATION>>> GetMessages(string user_id)
        {
            // TODO: 附带用户名
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            try {
                var result = await db
                    .Queryable<COMMUNICATION>()
                    .Where(it =>
                            it.SENDER_ID == userIdClaim && it.RECEIVER_ID == user_id
                            || it.SENDER_ID == user_id && it.RECEIVER_ID == userIdClaim)
                    .ToListAsync();

                if (result != null)
                    return Ok(result);
                else
                    return NotFound("未找到相关信息");
            }
            catch (Exception ex) {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        [HttpPost]
        [Route("send_message")]
        public async Task<ActionResult> SendMessages([FromBody] COMMUNICATION message)
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            try {
                var count = db.Insertable(message).ExecuteCommand();

                if (count > 0)
                    return Ok();
                else
                    return Forbid();
            }
            catch (Exception ex) {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }
    }
}