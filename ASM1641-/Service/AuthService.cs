using System;
using MongoDB.Driver;
using BookStore.Models;
using Microsoft.Extensions.Options;
using BookStore.Data;
using BookStore.IServices;
using BookStore.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace BookStore.Services
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
            var userExist = await _customerCollection.Find(e => true).ToListAsync();
            foreach(var e in userExist)
            {
                if (e.Email == request.Email)
                {
                    throw new Exception("This email has been existed!, please try another email...");
                }
            }

            string _passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
            request.PasswordHash = _passwordHash;
            await _customerCollection.InsertOneAsync(request);
        }

        public async Task<Customer> getCurrentUser(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParemeters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Application:Token").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParemeters, out validatedToken);

            // access claims from the validated principal

            var customerIdClaim = principal.FindFirst("customerId");
            if(customerIdClaim != null)
            {
                string customerId = customerIdClaim.Value;

                var customerFilter = Builders<Customer>.Filter.Eq("Id", customerId);
                var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

                return customer;
            }
            else
            {
                throw new Exception("User not found!");
            }
        }

        public async Task<string> LoginAsync(UserDto request)
        {
            var customersFilter = Builders<Customer>.Filter.Eq("Email", request.UserName);
            var customer = await _customerCollection.Find(customersFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("User not found, please check your user name again!");
            }

            if(!BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
            {
                throw new Exception("Wrong password!");
            }

            

            return customer.Email == "admin1@gmail.com" ? CreateToken(customer, "Admin") : CreateToken(customer, "User");
        }

        private string CreateToken(Customer aCustomer, string role)
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

