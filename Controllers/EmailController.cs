using UniTrade;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UniTrade.Tools;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
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

        [HttpPost("{address_type}")]
        public async Task<IActionResult> SendMailCode(string address_type)
        {
            /*前端需向后端发送  email@xx.com&type  字段*/
            //type包括注册（register）、登录（login）以及重置密码（findpwd），暂时仅有注册和登录功能

            string[] para = address_type.Split(new char[] { '&' });
            string address = para[0];
            string type = para[1];
            string mailName = address;      
            string title = "验证码";
            Random random = new();
            string Vericode = random.Next(100000, 999999).ToString();
            string bobyText = Vericode;
            if (type == "register")
            {
                bobyText = "注册验证码:" + Vericode + "，请勿向任何单位或个人泄露。若非本人操作,请忽略该邮件。感谢您对校易购的支持。".ToString();
                RegVeriCode = Vericode;
            }
            else if (type == "findpwd")
            {
                bobyText = "重置登录密码验证码:" + Vericode + "，请勿向任何单位或个人泄露。若非本人操作，请忽略该邮件。感谢您对校易购的支持。".ToString();
                FinVeriCode = Vericode;
            }
            else if (type == "login")
            {
                bobyText = "邮箱登录验证码:" + Vericode + "，请勿向任何单位或个人泄露，若非本人操作，请忽略该邮件。感谢您对校易购的支持。".ToString();
                LogVeriCode = Vericode;
            }
            // 使用smtp服务器 
            string host = "smtp.163.com";
            // 发送端账号   
            string userName = "hzd16992004@163.com";
            // 发送端授权码
            string pwd = "NPJRZHXXNJSRYUVN";
            MimeMessage message = new();
            //发件人
            message.From.Add(new MailboxAddress("校易购", userName));
            //收件人
            message.To.Add(new MailboxAddress(title, mailName));

            message.Subject = title;
            message.Body = new BodyBuilder
            {
                HtmlBody = bobyText
            }.ToMessageBody();
            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(host, 25, false);
                client.Authenticate(userName, pwd);
                await Task.Run(() => client.Send(message));
                client.Disconnect(true);

            }
            catch (Exception)
            {
                throw;
            }
            return Ok("OK");
        }
    }
}
