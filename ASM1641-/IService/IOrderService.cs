using ASM1641_.Dtos;
using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IOrderService
    {
        Task<OrderResult> GetAllOrdersAsync(int page);

        Task<Orders> GetOrderByIdAsync(string orderId);

        Task<OrderResult> GetOrdersByCustomerIdAsync(int page, string customerId);

        Task DeleteOrderAsync(string orderId);

        Task updateStatusOrder(string orderId, string status);
    }
}
