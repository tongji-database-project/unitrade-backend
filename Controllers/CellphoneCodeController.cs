using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace UniTrade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CellphoneCodeController : Controller
    {
        [HttpPost("{phone_type}")]
        public async Task<IActionResult> SendCellphoneCode(string phone_type)
        {
            // 分割参数
            string[] para = phone_type.Split('&');
            if (para.Length < 2)
            {
                return BadRequest("无效的 phone_type 参数");
            }

            string phone = para[0];
            string type = para[1];

            try
            {
                // 安全地获取凭证
                Credential cred = new()
                {
                    SecretId = Environment.GetEnvironmentVariable("TENCENT_CLOUD_SECRET_ID"),
                    SecretKey = Environment.GetEnvironmentVariable("TENCENT_CLOUD_SECRET_KEY")
                };

                ClientProfile clientProfile = new()
                {
                    SignMethod = ClientProfile.SIGN_TC3SHA256,
                    HttpProfile = new HttpProfile
                    {
                        ReqMethod = "POST",
                        Timeout = 10,
                        Endpoint = "sms.tencentcloudapi.com",
                        WebProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY")
                    }
                };

                SmsClient client = new(cred, "ap-guangzhou", clientProfile);

                Random random = new();
                string Vericode = random.Next(100000, 999999).ToString();

                // 根据类型设置相应的验证码
                switch (type)
                {
                    case "register":
                        EmailController.RegVeriCode = Vericode;
                        break;
                    case "findpwd":
                        EmailController.FinVeriCode = Vericode;
                        break;
                    case "login":
                        EmailController.LogVeriCode = Vericode;
                        break;
                    default:
                        return BadRequest("无效的 type 参数");
                }

                SendSmsRequest req = new()
                {
                    SmsSdkAppId = Environment.GetEnvironmentVariable("TENCENT_SMS_APP_ID"),
                    SignName = "校易购平台",
                    PhoneNumberSet = new string[] { "+86" + phone },
                    TemplateId = Environment.GetEnvironmentVariable("TENCENT_SMS_TEMPLATE_ID"),
                    TemplateParamSet = new string[] { Vericode }
                };

                SendSmsResponse resp = await client.SendSms(req);
                // 根据需要处理响应

                return Ok("验证码已成功发送。");
            }
            catch (Exception ex)
            {
                // 记录异常用于调试
                return StatusCode(500, $"内部服务器错误：{ex.Message}");
            }
        }
    }
}