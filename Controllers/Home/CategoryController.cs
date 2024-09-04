using Microsoft.AspNetCore.Mvc;

namespace UniTrade.Controllers.Home
{
    [ApiController]
    [Route("category")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("get")]
        public IActionResult GetStrings()
        {
            // 假设这是你的字符串数组
            var strings = new string[] { "居家", "美食", "服饰",};

            // 返回字符串数组
            return Ok(strings);
        }
    }
}