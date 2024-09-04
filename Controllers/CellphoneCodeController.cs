using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;

namespace UniTrade.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CellphoneCodeController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> SendCellphoneCode([FromQuery] string phone, [FromQuery] string type)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(type))
            {
                return BadRequest("手机号或请求类型为空");
            }

            try
            {
                // 安全地获取凭证
                Credential cred = new()
                {
                    SecretId = "AKIDRgmVDkKoBRacfZL3e3DF5lCh4DHCvQuK",
                    SecretKey = "SlCe2WrzBHH4d3GF0XyrfIiz2TikX6Rf"
                };

                ClientProfile clientProfile = new()
                {
                    SignMethod = ClientProfile.SIGN_TC3SHA256,
                    HttpProfile = new HttpProfile
                    {
                        ReqMethod = "POST",
                        Endpoint = "sms.tencentcloudapi.com",
                        WebProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY"),
                        Timeout = 5,
                    }
                };

                SmsClient client = new(cred, "ap-shanghai", clientProfile);

                Random random = new();
                string Vericode = random.Next(100000, 999999).ToString();

                string Template_id = string.Empty;
                // 根据类型设置相应的验证码
                switch (type)
                {
                    case "register":
                        EmailController.RegVeriCode = Vericode;
                        Template_id = "2256384";
                        break;
                    case "findpwd":
                        EmailController.FinVeriCode = Vericode;
                        Template_id = "2256389";
                        break;
                    case "login":
                        EmailController.LogVeriCode = Vericode;
                        Template_id = "2256382";
                        break;
                    default:
                        return BadRequest("无效的 type 参数");
                }

                SendSmsRequest req = new()
                {
                    SmsSdkAppid = "1400933429",
                    Sign = "TJ校易购公众号",
                    PhoneNumberSet = new string[] { "+86" + phone },
                    TemplateID = Template_id,
                    TemplateParamSet = new string[] { Vericode }
                };

                SendSmsResponse resp =  client.SendSmsSync(req);

                return Ok("验证码已成功发送。");
            }
            catch (Exception ex)
            {
                // 打印详细的异常信息
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                // 记录异常用于调试
                return StatusCode(500, $"内部服务器错误：{ex.Message}");
            }
        }
    }
}