using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Merchandise
{
    [Route("sellerInfo")]
    [ApiController]
    public class SellerInfoController : ControllerBase
    {
        [HttpGet("{merchandiseId}")]
        public async Task<IActionResult> GetSellerInfoByMerchandiseId(string merchandiseId)
        {
            /*SqlSugarClient db = Database.GetInstance();

            var sellerId = db.Queryable<SELLS>()
                              .Where(s => s.MERCHANDISE_ID == merchandiseId)
                              .Select(s => s.SELLER_ID)
                              .Single();

            if (sellerId == null)
            {
                return NotFound();
            }

            var sellerInfo = db.Queryable<USERS>()
                                .Where(u => u.USER_ID == sellerId)
                                .Select(u => new SellerInfo
                                {
                                    id = u.USER_ID,
                                    image = u.AVATAR
                                    name = u.NAME
                                    reputation = u.REPUTATION
                                })
                                .Single();

            return Ok(sellerInfo);*/

            return Ok(new SellerInfo
            {
                id = "2",
                image = "avatar.jpg",
                name = "张三",
                reputation = 78,
            });
        }
    }
}
