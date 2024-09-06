using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Org.BouncyCastle.Asn1.Ocsp;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;



namespace UniTrade.Controllers.User
{
    [Route("getMyOrder")]
    [ApiController]
    public class GetOrderNumContoller:ControllerBase
    {
        [HttpGet]
        public IActionResult getMyOrder()
        {
            SqlSugarClient db = Database.GetInstance();
            // 从 HTTP 请求中获取 token 中的 user_id 信息
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            try
            {
                var all = db.Queryable<USERS, PLACES, ORDERS>(
                    (user, place, order) => new object[] {
                       JoinType.Inner,user.USER_ID==place.CUSTOMER_ID,
                       JoinType.Inner,place.ORDER_ID==order.ORDER_ID
                    })
                    .Where(user => user.USER_ID == userIdClaim)
                    .Select((user, place, order) => new
                    {
                        user_id = user.USER_ID,
                        state = order.STATE,
                    })
                    .ToList();
                int all_num=all.Count();
                int recevied_num = all.Where(o => o.state == "ysh").Count();
                int transit_num = all.Where(o => o.state == "yfh").Count();
                int command_num = db.Queryable<USERS, PLACES, ORDERS, COMMENT_ON, COMMENTS>(
                    (user, place, order, comment_on, comment) => new object[] {
                       JoinType.Inner,user.USER_ID==place.CUSTOMER_ID,
                       JoinType.Inner,place.ORDER_ID==order.ORDER_ID,
                       JoinType.Inner,order.ORDER_ID==comment_on.ORDER_ID,
                       JoinType.Inner,comment_on.COMMENT_ID==comment.COMMENT_ID
                    })
                    .Where(user => user.USER_ID == userIdClaim)
                    .Count();  /*使用内连接，存在一个项就有一个已评论订单*/
                int uncommand_num = recevied_num - command_num;
                var response = new
                {
                    all=all_num,
                    received = recevied_num,
                    transit = transit_num,
                    uncommand=uncommand_num
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}