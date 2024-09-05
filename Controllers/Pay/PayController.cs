﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniTrade.Models;
using UniTrade.Tools;
using SqlSugar;
using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Domain;
using Aop.Api.Response;

namespace UniTrade.Controllers.Pay
{
    [Route("[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly SqlSugarClient _db;
        private readonly string _gatewayUrl;
        private readonly string _appId;
        private readonly string _merchantPrivateKey;
        private readonly string _alipayPublicKey;
        private readonly string _signType;
        private readonly string _charset;
        private readonly string _returnUrl;
        private readonly string _notifyUrl;

        public PayController()
        {
            _db = Database.GetInstance(); // 获取数据库实例

            // 从配置文件中读取支付宝参数
            _gatewayUrl = "https://openapi-sandbox.dl.alipaydev.com/gateway.do";
            _appId = "9021000140649027"; // 替换为你的支付宝App ID
            _merchantPrivateKey = "MIIEogIBAAKCAQEAguIjYblaz6mf4Rh0hrV4bFz3HilHnzUMiDTxaM7dwhWdyY5vJmyQZvSrDokrEycCM/rnOf089jzEtdrCu7rnEJz839/6xmp3N82hcXBydr3f3oXf/vIy6VUj73/Wmy1Znc2NTLnpff43kaNRJgW8csi1qQtpmo4BOTCb+bntsq2uT0FVUXJSASB/T4RJ50eDAQkbsV4RMV3q3f3sL904nmy7mDqktdUGpo1miDt3nb+mnPay1RbmpQfWx9NXTEac/t1jvqYyB6E7laglYXz7KES97gUz0ZPEFxvdnfrU9+gGmB46zMtc58XHO//piPPN0BnwXpd1QlkHUrFgnh6hUwIDAQABAoIBAAOMgCsZLIYu8j/XOumgKAjAKI8vzVpaxOE4lkciPM8TaPjbHNZs4Sl5ft3hCEL3rs6cAgMHg8ylbywDJ36RncxMhWrIlvMkVeE0eSkrRF9F3Lka5izygkDjZxsKW1ZPz3tA5JDtBZC+AOG5DO08AJUYLhzMS2u+Z/gWAlSuHuzWUHK94oWAcTwNJ2mFyt1uf4x8IkeUa5TWtm4iROUsyLUnHldjTxyBnPeB7MgwcefHH/0/USu+950LHdt38Or2HqFPRpEyJ8/RTWBKV7ZLhJA+3VAPeGqbjSSQzVxSCKtYJtQ5FeRUGH9qKIC/pUdz0TK5OQ3t8JGqtPo77cxT/akCgYEA8PxQfmmIVJCLNljSBP5n3ntNpzZB61b/vUkSf2Kw6/Z5xU+mecd1ghR524RNeSwaqRgMdVHseVbphYZE4NhRN2G01dzUl92cD/G5pl8df+m9WF+FQwr71Y9SOxG7m2R61oWH/2zkdgBYSHlW1sIvGENsSy5IL1UjHbw7ASaG3G0CgYEAiwm1cpUVPzSY0d7AnqDvD2tqF+O+Mg+g/AvQoAYfyGpQrQbK1vgQpbAOmmnkwqAsJ60QgDRX5iwoj2gxxkcmSwthVTKItIgTsWO4U/yTy/QhdrYBGKuDp6JyfMkg+SAOzTD/F+AerezzoTRy5tK2NYstUS2SxFOZguEZvFnNXL8CgYAy8UOa1XhKWBv1qyUKhTUz5ODvfhrFQyjgvLe6UgSQfRQUz9ejWrTOgMGZ1AgEm3wvTrQjgOP6apMG9PFOjqvshy0RIJOYXvKEtFbIgsbbqW+rZNXo7EN8S8XYUtFT8hO9kZDEQCKzhzMibvQahgYqnOYhpnEAAIFh4c6fWaOcqQKBgETvxWVQgb5a58J2/W4pQR6WFX5OzwinMxyFByWwo6WNAP8pNP0s9aQRDMyG4IOXQw1RA7NtQH/BOUMRWEyFDnl65QGHErzgE1OKq+QIgYdIPidLynNe5uIA617vokejG3OlbXV7C/sUVx+Aj8/efbVCkm+Ddxeck6qOzWHT7LylAoGASe/zM8rEFGOLEcoLMaETzFGAXC4Hhvg05NbdKdiU/AuPP58fbBHljrFL99s2Ra6NWBGlVOdRcx63AqiD9miKsB5uLyJG0QZNhPO2VkR4Uaxb9kvYFrTmkB3Y395NjpvFuITude6k8sFOHAW/iRAwu/06D0thKDqxZf4oEUnnobM="; // 替换为你的商户私钥
            _alipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0JekS8c2APkUc6JpM5jlOVi5BqQIId1AFCUNVlEWf3Q0Y43w3Tldowxx4RfrCI9kjrHGz525/z63UK9FIcu6AkaP7NdiMCnTcYbNt/ZOTFGda9AqEXKTAvmpQYQByczipUzw9WEtTfXoeOTttUZQVabecpHJ6THOdNOySIaRz61HXF3oi/rTc5t2WQFaDyYWtEyxHqAfvjpnygR8saUPmeH3vTTpRarwgFvw+exAV8YgpT3cCm0Qwz9D+TPNNpihAsnFf4kbbjDUV4OG9DMKqfqtM3UpZEeAvfaG+mi551kcPhFQcJW1XivZKnJgZUfXgbeJFlsNjNJWhxolDFt/kwIDAQAB"; // 替换为支付宝公钥
            _signType = "RSA2";
            _charset = "UTF-8";
            _returnUrl = "https://localhost:5173/payback";
            _notifyUrl = "https://localhost:5173/payback";
        }

        [HttpGet("create-payment")]
        public async Task<IActionResult> CreatePayment(string order_id)
        {
            // 验证订单ID是否存在
            if (string.IsNullOrEmpty(order_id))
            {
                return BadRequest("订单ID不能为空");
            }

            // 根据order_id查询订单信息
            var order = await GetOrderInfo(order_id);
            if (order == null)
            {
                //return NotFound("订单不存在");
            }

            try
            {
                // 创建支付宝客户端实例
                IAopClient client = new DefaultAopClient(
                    _gatewayUrl, _appId, _merchantPrivateKey,
                    "json", "1.0", _signType, _alipayPublicKey, _charset, false);

                // 构造支付请求对象
                AlipayOpenPublicTemplateMessageIndustryModifyRequest request = new AlipayOpenPublicTemplateMessageIndustryModifyRequest();

                var model = new AlipayTradeWapPayModel
                {
                       OutTradeNo = order_id,//order.ORDER_ID,
                       ProductCode = "QUICK_WAP_WAY",
                       TotalAmount = "12.02",//(order.ORDER_QUANITY * GetMerchandisePrice(order.MERCHANDISE_ID)).ToString("F2"),
                       Subject = "订单支付 - " + order_id,//order.ORDER_ID,
                       Body = "订单详情描述",
                       QuitUrl = _returnUrl // 支付中途退出返回商户网站地址
                };

                request.SetBizModel(model);

                AlipayOpenPublicTemplateMessageIndustryModifyResponse response = client.Execute(request);

                if (response.IsError)
                {
                    //return StatusCode(500, "创建支付请求失败");
                    return Ok("Ok");
                }
                else
                {
                    return Ok();
                }
                // 创建支付宝客户端实例
                //DefaultAopClient client = new DefaultAopClient(
                //    _gatewayUrl, _appId, _merchantPrivateKey,
                //    "json", "1.0", _signType, _alipayPublicKey, _charset, false);

                // 构造支付请求对象
                //AlipayTradeWapPayModel model = new AlipayTradeWapPayModel
                //{
                //    OutTradeNo = order.ORDER_ID,
                //    ProductCode = "QUICK_WAP_WAY",
                //    TotalAmount = (order.ORDER_QUANITY * GetMerchandisePrice(order.MERCHANDISE_ID)).ToString("F2"),
                //    Subject = "订单支付 - " + order.ORDER_ID,
                //    Body = "订单详情描述",
                //    QuitUrl = _returnUrl // 支付中途退出返回商户网站地址
                //};

                //AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
                //request.SetReturnUrl(_returnUrl + "?tradeNo=" + order.ORDER_ID); // 动态设置返回地址，包含订单号
                //request.SetNotifyUrl(_notifyUrl);
                //request.SetBizModel(model);

                // 执行请求，获取支付宝支付页面的URL
                //AlipayTradeWapPayResponse response = client.pageExecute<AlipayTradeWapPayResponse>(request, null, "post");

                //if (response != null)
                //{
                //    // 返回支付宝支付页面HTML代码
                //    return Content(response.Body, "text/html");
                //}
                //else
                //{
                //    return StatusCode(500, "创建支付请求失败");
                //}
                //return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }

        // 从数据库中获取订单信息
        private async Task<ORDERS> GetOrderInfo(string order_id)
        {
            try
            {
                var order = await _db.Queryable<ORDERS>()
                                     .Where(o => o.ORDER_ID == order_id)
                                     .FirstAsync();
                return order;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // 根据商品ID获取商品价格
        private decimal GetMerchandisePrice(string merchandiseId)
        {
            var merchandise = _db.Queryable<MERCHANDISES>()
                                 .Where(m => m.MERCHANDISE_ID == merchandiseId)
                                 .First();
            return merchandise.PRICE;
        }

        // 支付宝支付后的同步回调页面
        [HttpGet("return")]
        public IActionResult Return()
        {
            // 处理支付宝支付后的同步通知
            return Ok("支付成功");
        }

        // 支付宝支付后的异步回调页面
        [HttpPost("notify")]
        public IActionResult Notify()
        {
            // 处理支付宝支付后的异步通知
            return Ok();
        }


        /// <summary>
        /// 根据订单号获取订单的总金额。
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <returns>订单的总金额</returns>
        [HttpGet("get-order-total")]
        public async Task<ActionResult<decimal>> GetOrderTotal(string order_id)
        {
            try
            {
                // 使用联接查询ORDERS表和MERCHANDISES表
                var totalAmount = await _db.Queryable<ORDERS, MERCHANDISES>((o, m) => new JoinQueryInfos(
                                            JoinType.Inner, o.MERCHANDISE_ID == m.MERCHANDISE_ID))
                                           .Where(o => o.ORDER_ID == order_id)
                                           .SumAsync((o, m) => o.ORDER_QUANITY * m.PRICE);

                // 如果订单不存在
                if (totalAmount == null)
                {
                    return NotFound("订单不存在或订单中没有商品");
                }

                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器错误：{ex.Message}");
            }
        }
    }
}
