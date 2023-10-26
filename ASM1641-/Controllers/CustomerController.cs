using ASM1641_.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASM1641_.Controllers
{
    [Authorize]
    [Route("api/v1/customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerSerive;

        public CustomerController(ICustomerService _customerSerive)
        {
            this._customerSerive = _customerSerive;
        }


        // GET: api/values
        [HttpGet("viewcart"), Authorize(Roles = "User")]
        public async Task<IActionResult> Get()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
                string customerId = _customerSerive.GetIdByToken(token);
                var cart = await _customerSerive.ViewCartUser(customerId);

                return Ok(cart);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        //Add to cart
        [HttpPost("addtocart"), Authorize(Roles = "User")]
        public async Task<IActionResult> Post(string bookId, string quantity)
        {
            try
            {
                int quntity = int.Parse(quantity);
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
                string customerId = _customerSerive.GetIdByToken(token);
                if (string.IsNullOrEmpty(bookId) || quntity <= 0)
                {
                    return BadRequest("Error: Invalid input fields");
                }

                await _customerSerive.AddToCart(customerId, bookId, quntity);
                return Ok("Added item to cart successfully!");
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }

        // PUT api/values/5
        // update cart items
        [HttpPut("updatecart/{id}"), Authorize(Roles = "User")]
        public async Task<IActionResult> Put(string id, string quantity)
        {
            try
            {
                int quntity = int.Parse(quantity);
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token not found!");
                }

                if (string.IsNullOrEmpty(id) || quntity <= 0)
                {
                    return BadRequest($"Error: invalid inputs, quantity: {quantity}");
                }

                string customerId = _customerSerive.GetIdByToken(token);

                await _customerSerive.UpdateCartUser(customerId, id, quntity);
                return Ok("Updated cart items successfully");
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }

        // DELETE api/values/5
        //Remove item in cart

        [HttpDelete("delete-cart-item/{id}"), Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
                string customerId = _customerSerive.GetIdByToken(token);
                await _customerSerive.RemoveCartItems(customerId, id);

                return Ok("Deleted item successfully!");
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }

        [HttpPost("create-order"), Authorize(Roles = "User")]
        public async Task<IActionResult> CreateOder()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
                string customerId = _customerSerive.GetIdByToken(token);

                await _customerSerive.CreateOrder(customerId);
                return Ok("Created order successfully!");
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }


        [HttpGet("view-order-history"), Authorize(Roles = "User")]
        public async Task<IActionResult> ViewOrderHistory()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
                string customerId = _customerSerive.GetIdByToken(token);

                var orders = await _customerSerive.ViewOrdersHistory(customerId);

                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }
    }
}
