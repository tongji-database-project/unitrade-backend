using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniTrade.Tools;
using UniTrade.Models;
using UniTrade.ViewModel;
using SqlSugar;
using UniTrade.ViewModels;

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
                    .Distinct()
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
        public async Task<ActionResult<LatestMessageViewModel>> GetLatestMessage(string user_id)
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            try {
                var username = await db
                    .Queryable<USERS>()
                    .Where(it => it.USER_ID == user_id)
                    .Select(it => it.NAME)
                    .FirstAsync();

                if (username == null)
                    return NotFound("对方用户不存在");

                var content = await db
                    .Queryable<COMMUNICATION>()
                    .Where(it =>
                            it.SENDER_ID == userIdClaim && it.RECEIVER_ID == user_id
                            || it.SENDER_ID == user_id && it.RECEIVER_ID == userIdClaim)
                    .OrderBy(it => SqlFunc.Desc(it.COMMUNICATION_TIME))
                    .Select(it => it.COMMUNICATION_CONTENT)
                    .FirstAsync();

                if (content != null)
                    return Ok(new LatestMessageViewModel(username, content));
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
        public async Task<ActionResult<MessagesViewModel>> GetMessages(string user_id)
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            try {
                var username = await db
                    .Queryable<USERS>()
                    .Where(it => it.USER_ID == user_id)
                    .Select(it => it.NAME)
                    .FirstAsync();

                if (username == null)
                    return NotFound("对方用户不存在");

                var contents = db
                    .Queryable<COMMUNICATION>()
                    .Where(it =>
                            it.SENDER_ID == userIdClaim && it.RECEIVER_ID == user_id
                            || it.SENDER_ID == user_id && it.RECEIVER_ID == userIdClaim)
                    .OrderBy(it => it.COMMUNICATION_TIME)
                    .ToList()
                    .Select(it => SingleMessage.FromCommunication(it));

                if (contents != null)
                    return Ok(new MessagesViewModel(username, contents));
                else
                    return NotFound("未找到相关信息");
            }
            catch (Exception ex) {
                return StatusCode(500, $"信息获取时，服务器错误：{ex.Message}");
            }
        }

        [HttpPost]
        [Route("send_message")]
        public async Task<ActionResult> SendMessages([FromBody] SingleMessage message)
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            try {
                var mes = new COMMUNICATION();
                mes.SENDER_ID = message.sender;
                mes.RECEIVER_ID = message.receiver;
                mes.COMMUNICATION_CONTENT = message.content;
                mes.COMMUNICATION_TIME = message.time;
                var count = db.Insertable(mes).ExecuteCommand();

                if (count > 0)
                    return Ok();
                else
                    return Forbid();
            }
            catch (Exception ex) {
                Console.WriteLine($"{message.sender} send to {message.receiver} '{message.content}' at {message.time}: {ex}");
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }
    }
}