using SqlSugar;

// 仅供流程验证及参考使用，所有相关逻辑与行为与实际业务逻辑无关，
// 请勿在本文件中直接修改
// TODO: 在最终发行版中删除
// 这里绑定 ADMINISTRATORS 表为例
namespace UniTrade.Models
{
    [SugarTable("ADMINISTRATORS")]
    public class TestDemoModel
    {
        [SugarColumn(ColumnName = "ADMIN_ID", IsPrimaryKey = true)]
        public string Id { get; set; }
        [SugarColumn(ColumnName = "ADMIN_NAME", IsNullable = false)]
        public string Name {  get; set; }
        [SugarColumn(ColumnName = "ADMIN_PASSWORD", IsNullable = false)]
        public string Password {  get; set; } // 使用 SHA256 加密存储
        [SugarColumn(ColumnName = "ADMIN_LEVEL", IsNullable = false)]
        public int Level { get; set; }
    }
}
