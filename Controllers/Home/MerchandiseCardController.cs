using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using UniTrade.Tools;
using UniTrade.Models;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Home
{
    [Route("merchandiseCard")]
    [ApiController]
    public class MerchandiseController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMerchandiseById(string id)
        {
            SqlSugarClient db = Database.GetInstance();


            var merchandise = await db.Queryable<MERCHANDISES>()
                                      .Where(m => m.MERCHANDISE_ID == id)
                                      .SingleAsync();

            if (merchandise == null)
            {
                return NotFound();
            }

            return Ok(new MerchandiseCardInfo
            {
                name=merchandise.MERCHANDISE_NAME,
                price=merchandise.PRICE,
                image=merchandise.COVER_PICTURE_PATH
            });


            //测试用
            /*return Ok(new MerchandiseCardInfo
            {
                image="avatar.jpg",
                name="可乐",
                price=9900,
            });*/


        }
    }
}