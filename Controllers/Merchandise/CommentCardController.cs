using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Merchandise
{
    [ApiController]
    [Route("commentCard")]
    public class CommentCardController : ControllerBase
    {
        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetCommentDetails(string commentId)
        {
            SqlSugarClient db = Database.GetInstance();

            // Step 1: Query the comment details
            var comment = await db.Queryable<COMMENTS>()
                .Where(c => c.COMMENT_ID == commentId)
                .Select(c => new
                {
                    c.CONTENT,
                    c.COMMENT_TIME,
                })
                .SingleAsync();

            if (comment == null)
                return NotFound();

            IEnumerable<string> pic = await db.Queryable<COMMENTS_PICTURE>()
                .Where(c => c.COMMENT_ID == commentId)
                .Select(c => c.PICTURE_PATH)
                .ToListAsync();

            if (pic == null)
                return NotFound();

            // Step 2: Query the order ID from the COMMENT_ON relation
            var orderId = await db.Queryable<COMMENT_ON>()
                .Where(co => co.COMMENT_ID == commentId)
                .Select(co => co.ORDER_ID)
                .SingleAsync();

            if (orderId == null)
                return NotFound();

            // Step 3: Query the user ID from the PLACES relation
            var userId = await db.Queryable<PLACES>()
                .Where(p => p.ORDER_ID == orderId)
                .Select(p => p.CUSTOMER_ID)
                .SingleAsync();

            if (userId == null)
                return NotFound();

            // Step 4: Query the user details from the USERS table
            var user = await db.Queryable<USERS>()
                .Where(u => u.USER_ID == userId)
                .Select(u => new
                {
                    u.AVATAR,
                    u.NAME
                })
                .SingleAsync();

            if (user == null)
                return NotFound("userNotFound");

            var comment_scores = await db.Queryable<SCORES>()
                .Where(bg => bg.COMMENT_ID == commentId)
                .Select(bg => new
                {
                    bg.QUALITY,
                    bg.ATTITUDE,
                    bg.PRICE,
                    bg.LOGISTIC_SPEED,
                    bg.CONFORMITY
                })
                .SingleAsync();

            if (comment_scores == null)
            {
                return Ok(new CommentInfo
                {
                    content = comment.CONTENT,
                    time = comment.COMMENT_TIME,
                    pictures = pic,
                    user_avatar = user.AVATAR,
                    user_name = user.NAME,
                    quality = 6,
                    attitude = 6,
                    price = 6,
                    logistic_speed = 6,
                    conformity = 6
                });
            }
            // Return the combined comment details and user details

            return Ok(new CommentInfo
            {
                content = comment.CONTENT,
                time = comment.COMMENT_TIME,
                pictures = pic,
                user_avatar = user.AVATAR,
                user_name = user.NAME,
                quality=comment_scores.QUALITY,
                attitude = comment_scores.ATTITUDE,
                price = comment_scores.PRICE,
                logistic_speed = comment_scores.LOGISTIC_SPEED,
                conformity = comment_scores.CONFORMITY
            });


            /*return Ok(new CommentInfo
            {
                content = "VS白色的白色台南市",
                time = DateTime.Now,
                user_avatar = "avatar.jpg",
                user_name = "张三",
            });*/
        }
    }
}
