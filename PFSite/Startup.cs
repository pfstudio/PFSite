using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFSite.Data;
using PFSite.OAuth.GitHub;
using PFSite.Repositories;
using System;

namespace PFSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // System.Console.WriteLine(Configuration.GetConnectionString("MySQL"));

            // 注册数据库上下文
            services.AddDbContext<ApplicationDbContext>(options
                => options.UseMySql(Configuration.GetConnectionString("MySQL")));
            // 注册仓储对象
            services.AddScoped<RecordRepository>();
            services.AddScoped<UserRepository>();
            // 注册MVC
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // 注册TimeJob服务
            services.AddTimedJob();

            // 配置认证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGitHub(Configuration["github:clientId"], Configuration["github:clientSecret"]);

            // 配置XSRF Header
            services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // 确认数据库创建且存在
            // 此处无法确保表结构和代码定义一致
            // using (var scope = app.ApplicationServices.CreateScope())
            // {
            //     ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //     context.Database.EnsureCreated();
            // }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseTimedJob();

            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
