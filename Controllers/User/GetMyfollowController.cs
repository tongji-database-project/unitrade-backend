using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.User
{
    [Route("getMyfollow")]
    [ApiController]
    public class GetMyfollowController:ControllerBase
    {
        [HttpGet]
        public IActionResult Getfollow()
        {
            SqlSugarClient db = Database.GetInstance();
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            try
            {
                var test = db.Queryable<USERS, FOLLOWS, USERS>(
                        (buyer, follow, seller) => new object[] {
                            JoinType.Inner, buyer.USER_ID==follow.CUSTOMER_ID,
                            JoinType.Inner,follow.SELLER_ID==seller.USER_ID
                        }).Select((buyer, follow, seller) => new MyfollowViewModel
                        {
                            seller_name = seller.NAME,
                            seller_avator = seller.AVATAR,
                            seller_id = seller.USER_ID,
                            follow_time = follow.FOLLOW_TIME
                        }).ToList();
                var test_follow = db.Queryable<FOLLOWS>().ToList();
                foreach (var user in test)
                {
                    Console.WriteLine($"ID: {user.seller_id}, Name: {user.seller_name}, Ava: {user.seller_avator},time:{user.follow_time}");
                }

                var follows = db.Queryable<USERS,FOLLOWS,USERS>(
                        ( buyer,follow, seller) => new object[] {
                            JoinType.Inner, buyer.USER_ID==follow.CUSTOMER_ID,
                            JoinType.Inner,follow.SELLER_ID==seller.USER_ID
                        })
                    .Where((buyer, follow, seller) => buyer.USER_ID == id)
                    .Select((buyer, follow, seller) => new MyfollowViewModel
                    {
                        seller_name=seller.NAME,
                        seller_avator=seller.AVATAR,
                        seller_id=seller.USER_ID,
                        follow_time=follow.FOLLOW_TIME
                    })
                    .ToList();
                foreach (var user in follows)
                {
                    Console.WriteLine($"ID: {user.seller_id}, Name: {user.seller_name}, Ava: {user.seller_avator},time:{user.follow_time}");
                }
                return Ok(follows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
