using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IAuthService
    {
        Task CreateUserAsync(Customer request);
        Task<string> LoginAsync(string Username, string Password);
        Task<Customer> GetUser(string token);
    }
}
