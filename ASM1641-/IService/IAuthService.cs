using ASM1641_.Dtos;
using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IAuthService
    {
        Task CreateUserAsync(Customer request);
        Task<string> LoginAsync(UserDto request);
        Task<Customer> GetCurrentUser(string token);
    }
}
