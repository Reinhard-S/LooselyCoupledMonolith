using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sales;
using Shipping;
using System;
using System.Net.Http;

namespace AspNetCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSales();
            services.AddShipping();

            services.AddCap(options =>
            {
                options.ConsumerThreadCount = 0;
                options.UseInMemoryStorage();
                // options.UseRabbitMQ("localhost");
                options.UseAzureServiceBus(config =>
                {
                    config.ConnectionString = "Endpoint=sb://servida-dev-events.servicebus.windows.net/;SharedAccessKeyName=LooselyCoupledMonolithKey;SharedAccessKey=qMe+oZStvlmvUA9vrfhHXsCJGnAOjorumnuPtr/5x5M=";
                    config.TopicPath = "LooselyCoupledMonolith";
                });
                options.UseDashboard();
            });

            // Reinhard: Added to display the index page
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddControllersWithViews();

            // Reinhard: added a client to call the api endpoint from the UI
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient("ServerAPI", (provider, client) =>
            {
                var contextAccessor = provider.GetRequiredService<IHttpContextAccessor> ();
                client.BaseAddress = new Uri($"{contextAccessor.HttpContext.Request.Scheme}://{contextAccessor.HttpContext.Request.Host.ToUriComponent()}");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCapDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSales();
                endpoints.MapShipping();

                // Reinhard: Added to display the index page
                endpoints.MapControllerRoute("default", "{controller=Home}/action=Index/{id?}");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            // Reinhard: Required to load ~/_framework/blazor.server.js and handle events
            app.UseStaticFiles();
        }
    }
}
