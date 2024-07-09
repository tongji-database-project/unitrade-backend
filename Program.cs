using UniTrade.Tools;

namespace UniTrade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "codegen")
            {
                // ���ô���������
                Modelgen.codegen();
            }
            else
            {
                // ���� ASP.NET ������
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
