using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM1641_.Controllers
{
    [Authorize]
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/values
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GeGetAllCategoriest([FromQuery] string page)
        {
            var categories = await _categoryService.GetAllCategories(int.Parse(page));
            return Ok(categories);
        }

        // GET api/values/5
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> Get(string id)
        {
            var category = await _categoryService.GetByID(id);

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST api/values
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody]Category aCategory)
        {
            await _categoryService.CreateCategory(aCategory);

            return Ok(aCategory);
        }

        // PUT api/values/5
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(string id,string name)
        {
            var category = await _categoryService.GetByID(id);

            if(category == null)
            {
                return NotFound();
            }

            await _categoryService.UpdateCategory(name, id);

            return Ok("Updated successfully!");
        }

        // DELETE api/values/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var category = await _categoryService.GetByID(id);

            if (category == null)
            {
                return NotFound();
            }
            await _categoryService.RemoveCategory(id);
            return Ok();
        }
    }
}

