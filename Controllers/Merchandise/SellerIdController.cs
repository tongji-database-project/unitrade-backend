using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Merchandise
{
    [Route("sellerId")]
    [ApiController]
    public class SellerIdController : ControllerBase
    {
        [HttpGet("{merchandiseId}")]
        public async Task<IActionResult> GetSellerIdByMerchandiseId(string merchandiseId)
        {
            SqlSugarClient db = Database.GetInstance();

            var sellerId = await db.Queryable<SELLS>()
                              .Where(s => s.MERCHANDISE_ID == merchandiseId)
                              .Select(s => s.SELLER_ID)
                              .SingleAsync();

            if (sellerId == null)
            {
                return NotFound();
            }

            return Ok(sellerId);
        }
    }
}
