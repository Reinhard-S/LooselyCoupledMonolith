using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sales.Contracts;

namespace Sales
{
    public static class Endpoints
    {
        public static void MapSales(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/sales", async context =>
            {
                // create new order in the DB
                var dbContext = context.RequestServices.GetService<SalesDbContext>();

                // TODO - FIX ERROR: No database provider has been configured for this DbContext. A provider can be configured by overriding the DbContext.OnConfiguring method or by using AddDbContext on the application service provider. If AddDbContext is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object in its constructor and passes it to the base constructor for DbContext.
                // var orderId = dbContext.CreateOrder();  

                using (var reader = new StreamReader(context.Request.Body))
                {
                    /*
                    var body = reader.ReadToEnd();
                    var order = JsonSerializer.Deserialize<Order>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    */


                    // publish OrderCreate event
                    var events = context.RequestServices.GetService<Events>();

                    var orderPlaced = Task.Run(async () => await events.PublishOrderCreate(Guid.NewGuid())).GetAwaiter().GetResult();

                    await context.Response.WriteAsync($"Order {orderPlaced} has been placed.");
                };
            });

            endpoints.MapGet("/sales", async context => { await context.Response.WriteAsync("Hello Sales!"); });
        }



    }
}