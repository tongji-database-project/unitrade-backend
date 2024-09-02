using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace UniTrade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        public static string LogVeriCode = "000000";
        public static string RegVeriCode = "000000";
        public static string FinVeriCode = "000000";

        // 优化了接收参数
        [HttpPost]
        public async Task<IActionResult> SendMailCode([FromQuery] string address, [FromQuery] string type)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(type))
            {
                return BadRequest("Address or type cannot be null.");
            }

            string mailName = address;
            string title = "验证码";
            Random random = new();
            string Vericode = random.Next(100000, 999999).ToString();
            string bodyText = Vericode;

            switch (type.ToLower())
            {
                case "register":
                    bodyText = $"注册验证码: {Vericode}，请勿向任何单位或个人泄露。若非本人操作，请忽略该邮件。感谢您对校易购的支持。";
                    RegVeriCode = Vericode;
                    break;
                case "findpwd":
                    bodyText = $"重置登录密码验证码: {Vericode}，请勿向任何单位或个人泄露。若非本人操作，请忽略该邮件。感谢您对校易购的支持。";
                    FinVeriCode = Vericode;
                    break;
                case "login":
                    bodyText = $"邮箱登录验证码: {Vericode}，请勿向任何单位或个人泄露。若非本人操作，请忽略该邮件。感谢您对校易购的支持。";
                    LogVeriCode = Vericode;
                    break;
                default:
                    return BadRequest("Invalid type.");
            }

            string host = "smtp.163.com";
            string userName = "hzd16992004@163.com";
            string pwd = "NPJRZHXXNJSRYUVN";
            MimeMessage message = new();
            message.From.Add(new MailboxAddress("校易购", userName));
            message.To.Add(new MailboxAddress(title, mailName));
            message.Subject = title;
            message.Body = new BodyBuilder
            {
                HtmlBody = bodyText
            }.ToMessageBody();

            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(host, 25, false);
                client.Authenticate(userName, pwd);
                await Task.Run(() => client.Send(message));
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"邮件发送失败: {ex.Message}");
            }

            return Ok("验证码已发送");
        }
    }
}
