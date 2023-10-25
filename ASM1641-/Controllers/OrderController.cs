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
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string page)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(int.Parse(page));
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult>  GetById(string id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]string value)
        {
            try
            {
                await _orderService.updateStatusOrder(id, value);
                return Ok("Updated status order successfully!");
            }catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return Ok("Deleted order successfully!");
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}

