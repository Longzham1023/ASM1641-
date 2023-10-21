using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASM1641_.Controllers
{
    [Authorize]
    [Route("api/v1/publisher")]
    public class PublisherController : Controller
    {
        private readonly IPublisherService _publisherService;
        public PublisherController(IPublisherService publisherService)
        {
            this._publisherService = publisherService;
        }


        // GET: api/values
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            var publishers = await _publisherService.GetPublishers();
            return Ok(publishers);
        }

        // GET api/values/5
        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string id)
        {
            var publisher = await _publisherService.GetByID(id);

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }

        // POST api/values
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] Publisher aPublisher)
        {
            try
            {
                await _publisherService.CreatePublisher(aPublisher);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }

        }

        // PUT api/values/5
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(string id, [FromBody] Publisher aPublisher)
        {
            try
            {
                await _publisherService.UpdatePublisher(aPublisher, id);
                return Ok("Updated successfully!");
            }
            catch (Exception e)
            {
                return BadRequest($"An error occur: {e}");
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _publisherService.RemovePublisher(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e}");
            }

        }
    }
}
