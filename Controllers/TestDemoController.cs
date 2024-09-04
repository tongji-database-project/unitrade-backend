using Microsoft.AspNetCore.Mvc;
using UniTrade.Tools;
using UniTrade.Models;
using SqlSugar;
using System.Reflection.Metadata.Ecma335;

// 仅供流程验证及参考使用，请勿在本文件中直接修改
// TODO: 在最终发行版中删除
namespace UniTrade.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestDemoController : ControllerBase
    {
        // GET: /test
        [HttpGet]

        public async Task<ActionResult<IEnumerable<TestDemoModel>>> GetAdministrator()
        {
            SqlSugarClient db = Database.GetInstance();
            // TODO: 调试使用，用于打印调试信息
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" +
                db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            return await db.Queryable<TestDemoModel>().ToListAsync();
        }
    }
}
