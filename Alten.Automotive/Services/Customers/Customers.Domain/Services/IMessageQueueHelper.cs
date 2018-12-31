namespace Customers.Domain.Services
{
    public interface IMessageQueueHelper
    {
        bool PullMessageFromServiceBus(object state);
    }
}