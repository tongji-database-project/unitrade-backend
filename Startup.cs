using Microsoft.OpenApi.Models;
using System.Reflection;

namespace UniTrade
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 该方法会被运行时调用，可以使用该方法将服务加入到容器中
        public void ConfigureServices(IServiceCollection services)
        {
            // 解决跨域问题
            services.AddCors(options => options.AddPolicy(
                        name: "any",
                        configurePolicy: builder =>
                        {
                            builder.AllowAnyHeader()
                            .AllowAnyOrigin()
                            .AllowAnyMethod();
                        }));
            // 添加控制器
            services.AddControllers();
            // 添加 swagger 配置
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                        name: "v1",
                        info: new OpenApiInfo
                        {
                            Title = "UniTrade Api Swagger Doc",
                            Description = "try to use swagger build api doc",
                            Version = "v1"
                        });
                // 指定 XML 注释文件的位置（需要先在项目属性中开启文档生成）
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        // 该方法会被运行时调用，用于配置 HTTP 请求管道
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 加入 swagger 中间件
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "UNITRADE SWAGGER DOC");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // 允许跨域
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
// vim: set sw=4:
