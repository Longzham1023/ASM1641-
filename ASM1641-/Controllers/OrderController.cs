using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASM1641_.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM1641_.Controllers
{
    [Route("api/v1/order")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        public OrderController(IOrderService orderService, ICustomerService customerService)
        {
            _orderService = orderService;
            _customerService = customerService;
        }
        // GET: api/values
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromQuery] string page)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(int.Parse(page));
                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // GET api/values/5
        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            } catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // POST api/values
        [HttpPost, Authorize(Roles = "Admin")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(string id, [FromBody] string value)
        {
            try
            {
                await _orderService.updateStatusOrder(id, value);
                return Ok("Updated status order successfully!");
            } catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return Ok("Deleted order successfully!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpGet("get-order-by-customerid"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderByCustomerId([FromQuery]int page)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                string customerID = _customerService.GetIdByToken(token);
                var orders = await _orderService.GetOrdersByCustomerIdAsync(page, customerID);
                return Ok(orders);
            }
            catch (Exception e)
            { return BadRequest(e); }   
        }
    }
}

