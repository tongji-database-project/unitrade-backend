using SqlSugar;

namespace UniTrade.Tools
{
    public class Database
    {
        // 读取 JSON 配置文件
        private static IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // 读取配置文件下的连接字符串
        readonly static string connectString = Configuration.GetSection("ConnectionStrings")
            .GetSection("Oracle")
            .Value!;

        // 建立一个数据库连接实例
        public static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectString,
                // 数据库类型设置
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true,
            });
            return db;
        }
    }
}
// vim: set sw=4:
