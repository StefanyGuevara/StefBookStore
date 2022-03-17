using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BookStore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; set; }

        public Startup (IConfiguration temp)
        {
            Configuration = temp;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<BookstoreContext>(options =>
           {
               options.UseSqlite(Configuration["ConnectionStrings:BookDBConnection"]);
           });

            services.AddScoped<IBookStoreRepository, EFBookStoreRepository>();
            services.AddScoped<IOrderRepository, EFOrderRepository>();

            services.AddRazorPages();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddScoped<Basket>(x => SessionBasket.GetBasket(x));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddServerSideBlazor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // corresponds to the wwwroot
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {

            endpoints.MapControllerRoute("typepage",
                "{bookType}/Page{pageNum}",
                    new { Controller = "Home", action = "Index" });

                // it shows /Page1 for the page number of the url instead of question mark at the end. 
                endpoints.MapControllerRoute("Paging", "" +
                    "Page{pageNum}", 
                    new { Controller = "Home", action = "Index", pageNum = 1 });

                endpoints.MapControllerRoute("type",
                    "{bookType}",
                    new { Controller = "Home", action = "Index", pageNum = 1 });

            endpoints.MapDefaultControllerRoute();
                    

            endpoints.MapRazorPages();

            endpoints.MapBlazorHub();
                //tell them where to go
            endpoints.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");


            });
        }
    }
}
