using DotNetCore.CAP;
using Sales.Contracts;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sales
{
    public class Events: ICapSubscribe
    {
        private ICapPublisher capPublisher;

        private Channel<OrderPlaced> orderPlacedChannel = Channel.CreateUnbounded<OrderPlaced>();

        public Events(ICapPublisher capPublisher)
        {
            this.capPublisher = capPublisher;
        }

        public async Task<OrderPlaced> PublishOrderCreate(Guid orderId)
        {
            // publish the event
            var orderPlaced = new OrderPlaced
            {
                OrderId = orderId
            };

            await capPublisher.PublishAsync(nameof(OrderPlaced), orderPlaced, "OrderPlacedCallback");

            await orderPlacedChannel.Reader.WaitToReadAsync();
                return orderPlacedChannel.Reader.ReadAsync().Result;
        }   

        [CapSubscribe("OrderPlacedCallback")]
        public void OrderPlacedCallback(OrderPlaced orderPlaced)
        {
            orderPlacedChannel.Writer.WriteAsync(orderPlaced);
            orderPlacedChannel.Writer.Complete();
        }
    }
}
