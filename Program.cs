using UniTrade.Tools;

namespace UniTrade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "codegen")
            {
                // 调用代码生成器
                Modelgen.codegen();
            }
            else
            {
                // 启动 ASP.NET 服务器
                CreateDefaultBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateDefaultBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
// vim: set sw=4:
