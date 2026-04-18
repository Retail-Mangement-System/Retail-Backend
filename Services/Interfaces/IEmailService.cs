using RetailOrdering.API.Models;

namespace RetailOrdering.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(Order order);
    }
}
