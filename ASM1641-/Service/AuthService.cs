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
using Amazon.Runtime.Internal;
using System.Data;

namespace ASM1641_.Service
{
	public class AuthService : IAuthService
	{
        private readonly IMongoCollection<Customer> _customerCollection;
        private readonly IMongoCollection<Admin> _adminCollection;
        private readonly IOptions<DatabaseSetting> _dbSetting;
        private readonly IMongoCollection<StoreOwner> __storeOwnerCollection;
        private readonly IConfiguration _configuration;

        public AuthService(IOptions<DatabaseSetting> dbSetting, IConfiguration configuration)
		{
            _dbSetting = dbSetting;
            var mongoClient = new MongoClient(this._dbSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(this._dbSetting.Value.DatabaseName);

            _customerCollection = mongoDatabase.GetCollection<Customer>(this._dbSetting.Value.CustomerCollection);
            _configuration = configuration;
            _adminCollection = mongoDatabase.GetCollection<Admin>(this._dbSetting.Value.AdminCollection);
            __storeOwnerCollection = mongoDatabase.GetCollection<StoreOwner>(this._dbSetting.Value.StoreOwnerCollection);

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

            

            return CreateToken(customer, customer.Role);
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

        public async Task CreateAdminAccount(AdminRegisterDto Admin)
        {
            var isExist = await _customerCollection.Find(e => e.Email == Admin.Email).FirstOrDefaultAsync();
            if(isExist != null)
            {
                throw new Exception("An email has been existed!");
            }

            if (Admin == null || Admin.Email == null || Admin.password == null)
            {
                throw new Exception("Invalid input fields");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Admin.password);
            Admin.password = hashedPassword;
            Admin createAdmin = new Admin()
            {
                Email = Admin.Email,
                PasswordHash = Admin.password,
                Role = "Admin"
            };
            await _adminCollection.InsertOneAsync(createAdmin);
        }

        public async Task<string> loginAdminAsync(UserDto request)
        {
            var adminFilter = Builders<Admin>.Filter.Eq("Email", request.UserName);
            var admin = await _adminCollection.Find(adminFilter).FirstOrDefaultAsync();

            if (admin == null)
            {
                throw new Exception("Admin not found, please check your user name again!");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            {
                throw new Exception("Wrong password!");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", admin.Id),
                new Claim(ClaimTypes.Role, "Admin")
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

        public async Task<string> loginStoreOwnerAsync(UserDto request)
        {
            var storeOwnerFilter = Builders<StoreOwner>.Filter.Eq("Email", request.UserName);
            var storeOwner = await __storeOwnerCollection.Find(storeOwnerFilter).FirstOrDefaultAsync();

            if (storeOwner == null)
            {
                throw new Exception("Store owner not found, please check your user name again!");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, storeOwner.PasswordHash))
            {
                throw new Exception("Wrong password!");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", storeOwner.Id),
                new Claim(ClaimTypes.Role, "StoreOwner")
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

        public async Task ResetPasswordUser(ChangePasswordDto request)
        {
            var userFilter = Builders<Customer>.Filter.Eq("Email", request.Email);

            var isExist = await _customerCollection.Find(userFilter).FirstOrDefaultAsync();
            if(isExist == null)
            {
                throw new Exception("Email user not found!");
            }

            string newPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            isExist.PasswordHash = newPassword;
            await _customerCollection.ReplaceOneAsync(userFilter, isExist);
        }


        public async Task ChangePasswordUser(string customerId, string password)
        {
            var customerFilter = Builders<Customer>.Filter.Eq("Id", customerId);
            var user = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found!");
            }

            string newPassowrd = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = newPassowrd;
            await _customerCollection.ReplaceOneAsync(customerFilter, user);
        }

        public string GetIdByToken(string token)
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


            var customerIdClaim = principal.FindFirst("customerId");
            if (customerIdClaim != null)
            {
                string customerId = customerIdClaim.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return "Unknown ID";
                }

                return customerId;
            }
            else
            {
                throw new Exception("User not found!");
            }
        }
    }
}

