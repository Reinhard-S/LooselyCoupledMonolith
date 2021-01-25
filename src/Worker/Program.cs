﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sales;
using Shipping;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSales();
                    services.AddShipping();

                    services.AddCap(options =>
                    {
                        options.UseInMemoryStorage();
                        // options.UseRabbitMQ("localhost");
                        options.UseAzureServiceBus(config =>
                        {
                            config.ConnectionString = "Endpoint=sb://servida-dev-events.servicebus.windows.net/;SharedAccessKeyName=LooselyCoupledMonolithKey;SharedAccessKey=qMe+oZStvlmvUA9vrfhHXsCJGnAOjorumnuPtr/5x5M=";
                            config.TopicPath = "LooselyCoupledMonolith";
                        });
                    });

                });
    }
}
