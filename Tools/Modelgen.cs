namespace UniTrade.Tools
{
    public class Modelgen
    {
        public static void codegen()
        {
            Console.WriteLine("正在生成实体代码，耗时较长请耐心等待...");

            // 建立数据库连接
            var db = Database.GetInstance();

            // 生成实体代码
            db.DbFirst.IsCreateDefaultValue()
                .IsCreateDefaultValue()
                .Where(table => true)
                .CreateClassFile(@".\Models", "UniTrade.Models");
            
            // 生成完成后打印提示信息
            Console.WriteLine("生成完成。");
        }
    }
}
