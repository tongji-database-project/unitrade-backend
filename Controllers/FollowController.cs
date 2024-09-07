using Microsoft.AspNetCore.Mvc;
using UniTrade.Models;
using SqlSugar;
using System;
using System.Security.Claims;
using UniTrade.Tools;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Principal;
using System.Threading.Tasks;
using UniTrade.ViewModels;

namespace UniTrade.Controllers
{
    [Route("FollowController")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        // 1. 关注卖家
        [Authorize]
        [HttpPost("follow")]
        public IActionResult FollowSeller([FromBody] FollowRequest request)
        {
            SqlSugarClient db = Database.GetInstance();

            try
            {
                // 获取当前用户 ID
                var customerId = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("用户未登录");
                }

                // 检查是否已经关注
                var isFollowing = db.Queryable<FOLLOWS>()
                    .Where(f => f.CUSTOMER_ID == customerId && f.SELLER_ID == request.SellerId)
                    .Any();

                if (isFollowing)
                {
                    return BadRequest("已关注该卖家");
                }

                // 插入关注信息
                FOLLOWS follow = new FOLLOWS
                {
                    CUSTOMER_ID = customerId,
                    SELLER_ID = request.SellerId,
                    FOLLOW_TIME = DateTime.Now
                };

                db.Insertable(follow).ExecuteCommand();
                return Ok("关注成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"内部错误: {ex.Message}");
            }
        }

        // 2. 取消关注卖家
        [Authorize]
        [HttpPost("unfollow")]
        public IActionResult UnfollowSeller([FromBody] FollowRequest request)
        {
            SqlSugarClient db = Database.GetInstance();

            try
            {
                // 获取当前用户 ID
                var customerId = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("用户未登录");
                }

                // 删除关注记录
                var result = db.Deleteable<FOLLOWS>()
                    .Where(f => f.CUSTOMER_ID == customerId && f.SELLER_ID == request.SellerId)
                    .ExecuteCommand();

                if (result > 0)
                {
                    return Ok("取消关注成功");
                }
                else
                {
                    return NotFound("未找到相关关注记录");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"内部错误: {ex.Message}");
            }
        }

        // 3. 检查是否已关注卖家
        [Authorize]
        [HttpGet("isFollowing/{sellerId}")]
        public IActionResult IsFollowing(string sellerId)
        {
            SqlSugarClient db = Database.GetInstance();

            try
            {
                // 获取当前用户 ID
                var customerId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("用户未登录");
                }

                // 检查是否存在关注关系
                var isFollowing = db.Queryable<FOLLOWS>()
                    .Where(f => f.CUSTOMER_ID == customerId && f.SELLER_ID == sellerId)
                    .Any();

                return Ok(new { isFollowing });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"内部错误: {ex.Message}");
            }
        }
    }

    // 请求体模型
    public class FollowRequest
    {
        public string SellerId { get; set; }
    }
}
