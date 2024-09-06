using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniTrade.Tools;
using UniTrade.Models;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Home
{
    [ApiController]
    [Route("merchandiseId")]
    public class MerchandiseIdController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;

        //public MerchandiseIdController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        [HttpGet("get-id")]
        public async Task<IActionResult> GetRandomId()
        {
            SqlSugarClient db = Database.GetInstance();

            const int count = 42;
             //从数据库中获取商品ID并随机挑选指定数量的商品ID
            var productIds = db.Queryable<MERCHANDISES>()
                               .Select(p => p.MERCHANDISE_ID)
                               .ToList();

            Random random = new Random();
            return Ok(productIds.OrderBy(x => random.Next()).Take(count).ToList());



            //测试用 
            //var merchandise_id_list = new string[] { "1", "2", "3", "4","5","6","7","8","1", "1", "2", "3", "4", "5", "6", "7", "8", "1", "1", "2", "3", "4", "5", "6", "7", "8", "1", "1", "2", "3", "4", "5", "6", "7", "8", "1", "1", "2", "3", "4", "5", "6", "7", "8", "1" };

            //return Ok(merchandise_id_list);
        }

        [HttpPost("getSpecialId")]
        public async Task<IActionResult> GetSpecialId([FromBody] GetSpecialIDViewModel model)
        {
            SqlSugarClient db = Database.GetInstance();

            if (string.IsNullOrEmpty(model.SpecialName))
            {
                return BadRequest("输入不能为空");
            }

            var productIds = db.Queryable<MERCHANDISES>()
                               .Where(p => p.MERCHANDISE_NAME.Contains(model.SpecialName))
                               .Select(p => p.MERCHANDISE_ID)
                               .ToList();

            if (productIds == null || productIds.Count == 0)
            {
                return NotFound("未找到包含指定名称的商品");
            }

            return Ok(productIds);
        }
    }
}
