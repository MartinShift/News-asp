namespace News.Models
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using News.DbModels;

    namespace YourProjectName
    {
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.Configure<PasswordHasherOptions>(opt => opt.IterationCount = 10000);
                services.AddMvc();
                services.AddDbContext<NewsDbContext>((options) =>
                {
                    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                    options.UseSqlite("Data Source=D:\\Mein progectos\\News\\News\\NewsDb.db");
                });
                services.AddControllersWithViews();
                services.AddIdentity<User, IdentityRole<int>>(options =>
                {
                    options.Password.RequiredLength = 8; 
                    options.Password.RequireNonAlphanumeric = false; 
                  
                })
    .AddEntityFrameworkStores<NewsDbContext>()
    .AddDefaultTokenProviders();
                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(120); 
                });
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
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
                        pattern: "{controller=News}/{action=Index}/{id?}");
                    endpoints.MapControllerRoute(
                        name: "NewsDetail",
                        pattern: "News/Detail/{id}",
                        defaults: new { controller = "News", action = "NewsDetail" });
                    endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Map}/{action=Index}/{id?}");
                    endpoints.MapControllerRoute(
                name: "TodoList",
                pattern: "{controller=Todo}/{action=Index}",
            defaults: new { controller = "Todo", action = "Index" });
                endpoints.MapControllerRoute(
                        name: "Edit",
                        pattern: "News/Edit",
                        defaults: new { controller = "News", action = "Edit" }
                        );

                });

            }
        }
    }

}
