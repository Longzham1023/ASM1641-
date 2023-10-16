using ASM1641_.Dtos;
using ASM1641_.IService;
using ASM1641_.Models;
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


        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
