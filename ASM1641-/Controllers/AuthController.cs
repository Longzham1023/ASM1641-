using ASM1641_.Dtos;
using ASM1641_.IService;
using ASM1641_.Models;
using ASM1641_.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM1641_.Controllers
{
    [Route("api/v1/authenticate")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Customer request)
        {
            try
            {
                await _authService.CreateUserAsync(request);
                return Ok("User created successfully!");
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e.Message}");
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto request)
        {
            try
            {
                if (request.UserName == null || request.Password == null)
                {
                    return BadRequest("Invalid username or password!");
                }

                string token = await _authService.LoginAsync(request);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e.Message}");
            }
        }


        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;


                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest($"Invalid token!");
                }

                var customer = await _authService.GetCurrentUser(token);
                return Ok(customer);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] UserDto request)
        {
            try
            {
                if (request.UserName == null || request.Password == null)
                {
                    return BadRequest("Invalid username or password!");
                }

                string token = await _authService.loginAdminAsync(request);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e.Message}");
            }
        }

        [HttpPost("store-owner-login")]
        public async Task<IActionResult> StoreOwnerLogin([FromBody] UserDto request)
        {
            try
            {
                if (request.UserName == null || request.Password == null)
                {
                    return BadRequest("Invalid username or password!");
                }

                string token = await _authService.loginStoreOwnerAsync(request);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e.Message}");
            }
        }

        [HttpPost("create-admin-account")]
        public async Task<IActionResult> CreateAdmin([FromForm] AdminRegisterDto Admin)
        {
            try
            {
                await _authService.CreateAdminAccount(Admin);
                return Ok("Created account for admin successfully!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut("reset-password-user"), AllowAnonymous]
        public async Task<IActionResult> ResetPasswordUser([FromForm]ChangePasswordDto request)
        {
            try
            {
                await _authService.ResetPasswordUser(request);
                return Ok("Reset password successfully!");
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut("change-password-user"), Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePasswordUser(string password)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
                string customerId = _authService.GetIdByToken(token);
                await _authService.ChangePasswordUser(customerId, password);
                return Ok("Changed password successfully!");
            }catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
