using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface ICustomerService
    {
        Task UpdateCustomer(Customer aCustomer, string Id);

        Task<IEnumerable<Orders>> ViewOrdersHistory(string customerId);

        Task<List<CartItems>> ViewCartUser(string CustomerId);

        Task AddToCart(string CustomerId, string BookId, int quantity);

        Task UpdateCartUser(string customerId, string BookId, int quantity);

        Task RemoveCartItems(string CustomerId, string BookId);

        Task CreateOrder(string customerId);

        string GetIdByToken(string token);
    }
}
