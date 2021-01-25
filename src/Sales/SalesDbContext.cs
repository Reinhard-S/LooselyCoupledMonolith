using Microsoft.EntityFrameworkCore;
using System;

namespace Sales
{
    public class SalesDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }


        public Guid CreateOrder()
        {
            var order = Add(new Order()
            {
                OrderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid()
            });

            return order.Entity.OrderId;
        }
    }
}