﻿

namespace WebApplication4
{
	public class Startup
	{
       
            public IConfiguration Configuration { get; }
            public Startup(IConfiguration configuration)
            {
                this.Configuration = configuration;
            }

            // This method gets called by the runtime. Use this method to add serices to the container.
            public void ConfigureServices(IServiceCollection services)
            {
            
            services.Configure<RouteOptions>(routeOptions =>
            {
                routeOptions.LowercaseUrls = true;
            });
                services.AddControllersWithViews();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(WebApplication app, IWebHostEnvironment env)
            {
                if (!env.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.UseEndpoints(endpoints =>
                {
                    // Thêm endpoint chuyển đến các trang Razor Page
                    // trong thư mục Pages
                    endpoints.MapRazorPages();
                });
        }
    }
    
}
