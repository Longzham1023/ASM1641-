using ASM1641_.Data;
using ASM1641_.Dtos;
using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ASM1641_.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IMongoCollection<Customer> _customerCollection;
        private readonly IOptions<DatabaseSetting> _dbSetting;
        private readonly IMongoCollection<Book> _bookCollection;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Orders> _orderCollection;

        public CustomerService(IOptions<DatabaseSetting> dbSetting, IConfiguration configuration)
        {
            _dbSetting = dbSetting;

            var mongoClient = new MongoClient(this._dbSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(this._dbSetting.Value.DatabaseName);

            _customerCollection = mongoDatabase.GetCollection<Customer>(this._dbSetting.Value.CustomerCollection);
            _bookCollection = mongoDatabase.GetCollection<Book>(this._dbSetting.Value.BooksCollection);
            _configuration = configuration;
            _orderCollection = mongoDatabase.GetCollection<Orders>(this._dbSetting.Value.OrderCollection);
        }

        //done
        public async Task<CartDto> AddToCart(string CustomerId, string BookId, int quantity)
        {
            if (string.IsNullOrEmpty(CustomerId) || string.IsNullOrEmpty(BookId) || quantity <= 0)
            {
                throw new Exception("Invalid input fields");
            }

            //Filter customer
            var customerFilter = Builders<Customer>.Filter.Eq("Id", CustomerId);
            var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();


            //Filter book
            var bookFilter = Builders<Book>.Filter.Eq("Id", BookId);
            var book = await _bookCollection.Find(bookFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("Customer not found!");
            }

            if (book == null)
            {
                throw new Exception("Book not found!");

            }

            //Check for Cart user
            if (customer.CartItems == null)
            {
                customer.CartItems = new List<CartItems>();
            }

            // Check if item is existing in CartItems or not?
            bool isExist = false;
            CartItems item = new CartItems();
            foreach (var e in customer.CartItems)
            {
                if (e.BookId.ToString().Equals(BookId))
                {
                    isExist = true;
                    item = e;
                    break;
                }
            }

            if (isExist == true)
            {
                //update quantity
                customer.CartItems.Remove(item);
                item.Quantity += quantity;
                customer.CartItems.Add(item);
            }
            else
            {
                // Add new items
                item.BookId = book.Id;
                item.Quantity = quantity;
                item.Price = book.Price;
                item.imagePath = book.ImagePath;
                customer.CartItems.Add(item);
            }


            // update CartItems current

            await _customerCollection.ReplaceOneAsync(customerFilter, customer);
            return new CartDto()
            {
                id = item.BookId,
                quantity = item.Quantity,
                imagePath = item.imagePath,
                message = "Added book to cart successfully!",
                price = item.Price
            };
        }

        public async Task CreateOrder(string customerId)
        {
            var customerFilter = Builders<Customer>.Filter.Eq("Id", customerId);
            var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("Error: Customer not found!");
            }

            if (customer.CartItems != null)
            {
                DateTime date = DateTime.Now;

                // calculate totalAmount

                decimal totalAmount = 0;

                foreach (var e in customer.CartItems)
                {
                    decimal totalPerItem = e.Price * e.Quantity;
                    totalAmount += totalPerItem;
                }


                Orders order = new Orders
                {
                    OrderDate = date,
                    TotalAmount = totalAmount,
                    OrderItems = customer.CartItems,
                    customerId = customer.Id,
                    address = customer.Address,
                    phoneNumber = customer.Phone
                };

                await _orderCollection.InsertOneAsync(order);
                customer.CartItems.Clear();
                await _customerCollection.ReplaceOneAsync(customerFilter, customer);
            }
            else
            {
                throw new Exception("Cart items is empty!");
            }

        }
        //done
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
        //done
        public async Task RemoveCartItems(string CustomerId, string BookId)
        {
            //Filter customer
            var customerFilter = Builders<Customer>.Filter.Eq("Id", CustomerId);
            var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("Customer not found!");
            }

            if (customer.CartItems != null)
            {
                foreach (var e in customer.CartItems)
                {
                    if (e.BookId.ToString().Equals(BookId))
                    {
                        customer.CartItems.Remove(e);
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Cart empty!");
            }

            await _customerCollection.ReplaceOneAsync(customerFilter, customer);
        }
        //done
        public async Task UpdateCartUser(string customerId, string BookId, int quantity)
        {
            var customerFilter = Builders<Customer>.Filter.Eq("Id", customerId);
            var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("User not found!");
            }

            if (customer.CartItems == null)
            {
                throw new Exception("Update failed, cart is empty");
            }
            else
            {
                bool isexits = false;
                CartItems item = new CartItems();
                foreach (var e in customer.CartItems)
                {
                    if (e.BookId.ToString().Equals(BookId))
                    {
                        isexits = true;
                        item = e;
                    }
                }

                if (!isexits)
                {
                    throw new Exception("Item is not existing in card!");
                }
                else
                {
                    customer.CartItems.Remove(item);
                    item.Quantity = quantity;
                    customer.CartItems.Add(item);
                }
            }

            await _customerCollection.ReplaceOneAsync(customerFilter, customer);
        }

        //done
        public async Task UpdateCustomer(Customer aCustomer, string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                throw new Exception("Invalid ID");
            }

            var customerFilter = Builders<Customer>.Filter.Eq("Id", Id);
            var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            if (customer != null)
            {
                customer.FirstName = aCustomer.FirstName;
                customer.LastName = aCustomer.LastName;
                customer.Address = aCustomer.Address;
                customer.Phone = aCustomer.Phone;

                // replace to new customer
                await _customerCollection.ReplaceOneAsync(customerFilter, customer);
            }
        }
        //done
        public async Task<List<CartItems>> ViewCartUser(string CustomerId)
        {
            // Filter customer
            var customerFilter = Builders<Customer>.Filter.Eq("_id", ObjectId.Parse(CustomerId)); // Assuming _id is of type ObjectId
            var customer = await _customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                throw new Exception("Customer not found!");
            }

            // Check for Cart user
            if (customer.CartItems == null)
            {
                customer.CartItems = new List<CartItems>();
            }

            return customer.CartItems;

        }

        public async Task<IEnumerable<Orders>> ViewOrdersHistory(string customerId)
            => await _orderCollection.Find(e => e.customerId == customerId).ToListAsync();
    }
}
