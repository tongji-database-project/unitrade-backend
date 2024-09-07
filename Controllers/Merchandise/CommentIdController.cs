using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniTrade.Tools;
using UniTrade.Models;

namespace UniTrade.Controllers.Merchandise
{
    [ApiController]
    [Route("commentId")]
    public class CommentIdController : ControllerBase
    {
        [HttpGet("{merchandiseId}")]
        public async Task<IActionResult> GetCommentId(string merchandiseId)
        {
            SqlSugarClient db = Database.GetInstance();

            var commentIds = db.Queryable<COMMENT_ON>()
                .Where(c => c.MERCHANDISE_ID == merchandiseId)
                .Select(c => c.COMMENT_ID)
                .ToList();

            return Ok(commentIds);



            //测试用 
            //var comment_id_list = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "1", "1", "2", "3", "4", "5", "6", "7", "8", "1", "1", "2", "3", "4", "5", "6", "7", "8", "1", "1", "2", "3", "4", "5", "6", "7", "8", "1", "1", "2", "3", "4", "5", "6", "7", "8", "1" };

            //return Ok(comment_id_list);
        }
    }
}
