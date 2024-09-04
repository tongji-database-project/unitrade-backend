using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniTrade.Tools;
using UniTrade.Models;

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
    }
}
