using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoMVCAppAsFastAsICan.Data;

namespace TodoMVCAppAsFastAsICan
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
            services.AddAuthentication("Auth").AddCookie("Auth", cookieConfig =>
            {
                cookieConfig.LoginPath = "/Account/Login";
                cookieConfig.Cookie.Name = "Auth.cookie";
                cookieConfig.AccessDeniedPath = "/Account/Login";
            });

            services.AddAuthorization(authConfig =>
            {
                authConfig.AddPolicy("Auth_Policy", policyBuilder =>
                {
                    policyBuilder.RequireClaim(ClaimTypes.Email);
                    policyBuilder.RequireClaim(ClaimTypes.Name)
                });
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSingleton<MongoDbAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Todo}/{action=Index}/{id?}");
            });
        }
    }
}
