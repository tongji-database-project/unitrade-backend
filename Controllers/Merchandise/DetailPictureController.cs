using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;
using UniTrade.Models;
using UniTrade.Tools;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Merchandise
{
    [ApiController]
    [Route("detailPicture")]
    public class DetailPictureController : ControllerBase
    {
        [HttpGet("{merchandiseId}")]
        public async Task<IActionResult> GetDetailPicture(string merchandiseId)
        {
            SqlSugarClient db = Database.GetInstance();

            var pictures = db.Queryable<MERCHANDISES_PICTURE>()
                  .Where(p => p.MERCHANDISE_ID == merchandiseId)
                  .Select(p => p.PICTURE_PATH)
                  .ToList();

            if (pictures == null)
                return NotFound();

            return Ok(pictures);

            /*var merchandise_id_list = new string[] { "logo.png", "logo.png", "logo.png" };
            return Ok(merchandise_id_list);*/
        }
    }
}
