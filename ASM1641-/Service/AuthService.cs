using System;
using MongoDB.Driver;
using ASM1641_.Models;
using ASM1641_.Dtos;
using Microsoft.Extensions.Options;
using ASM1641_.Data;
using ASM1641_.IService;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace ASM1641_.Service
{
	public class AuthService : IAuthService
	{
        public readonly IMongoCollection<Customer> _customerCollection;
        public readonly IOptions<DatabaseSetting> _dbSetting;
        private readonly IConfiguration _configuration;

        public AuthService(IOptions<DatabaseSetting> dbSetting, IConfiguration configuration)
		{
            _dbSetting = dbSetting;
            var mongoClient = new MongoClient(this._dbSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(this._dbSetting.Value.DatabaseName);

            _customerCollection = mongoDatabase.GetCollection<Customer>(this._dbSetting.Value.CustomerCollection);
            _configuration = configuration;

        }

        public async Task CreateUserAsync(Customer request)
        {
            var existingUser = await _customerCollection.Find(e => e.Email == request.Email).FirstOrDefaultAsync();

            if (existingUser != null)
            {
                throw new Exception("This email has already been used. Please try another email.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
            request.PasswordHash = hashedPassword;

            await _customerCollection.InsertOneAsync(request);
        }
        public async Task<string> LoginAsync(UserDto request)
        {
            var customersFilter = Builders<Customer>.Filter.Eq("Email", request.UserName);
            var customer = await _customerCollection.Find(customersFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("User not found, please check your user name again!");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
            {
                throw new Exception("Wrong password!");
            }



            return customer.Email == "admin1@gmail.com" ? CreateToken(customer, "Admin") : CreateToken(customer, "User");
        }

        public async Task<Customer> GetCurrentUser(string token)
        {
            var principal = ValidateTokenAndGetPrincipal(token);

            var customerIdClaim = principal.FindFirst("customerId");

            if (customerIdClaim == null)
            {
                throw new Exception("User not found!");
            }

            string customerId = customerIdClaim.Value;
            var customerFilter = Builders<Customer>.Filter.Eq("Id", customerId);
            var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("User not found!");
            }

            return customer;
        }

        private ClaimsPrincipal ValidateTokenAndGetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Application:Token").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

            return principal;
        }
        


        public string CreateToken(Customer aCustomer, string role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, aCustomer.FirstName),
                new Claim(ClaimTypes.Email, aCustomer.Email),
                new Claim("CustomerId", aCustomer.Id),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Application:Token").Value!
                ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}

