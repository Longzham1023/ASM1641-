using ASM1641_.Dtos;
using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IAuthService
    {
        Task CreateUserAsync(Customer request);
        Task<string> LoginAsync(UserDto request);
        Task<Customer> GetCurrentUser(string token);
        Task CreateAdminAccount(AdminRegisterDto Admin);
        Task<string> loginAdminAsync(UserDto request);
        Task<string> loginStoreOwnerAsync(UserDto request);
        Task ResetPasswordUser(ChangePasswordDto request);
        Task ChangePasswordUser(string customerId, string password);
        string GetIdByToken(string token);
    }
}
