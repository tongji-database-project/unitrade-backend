using Microsoft.OpenApi.Models;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UniTrade.Tools;

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

            // 添加身份认证配置
            // services.AddAuthentication(options =>
            //         {
            //         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //         })
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                    {
                    var secrectByte = Encoding.UTF8.GetBytes(TokenParameter.SecretKey);

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                    // 验证 token 颁发者
                    ValidateIssuer = true,
                    ValidIssuer = TokenParameter.Issuer,

                    // 验证接收者
                    ValidateAudience = true,
                    ValidAudience = TokenParameter.Audience,

                    // 对签名的 SecurityToken 的 SecurityKey 进行验证
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secrectByte),

                    // 验证失效时间
                    ValidateLifetime = true,
                    };
                    });

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

            // 配置身份认证与授权中间件
            // 身份认证中间件需要在所有需要身份认证的中间件前调用（如授权中间件）
            app.UseAuthentication();
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
