using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.AspNetCore.Authorization;


namespace ASM1641_.Controllers
{
    [Route("api/v1/authors")]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAuthors([FromQuery] string page)
        {
            try
            {
                var authors = await _authorService.GetAllAuthors(int.Parse(page));
                return Ok(authors);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthorById(string id)
        {
            var author = await _authorService.GetByID(id);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAuthor([FromBody] Author anAuthor)
        {
            await _authorService.CreateAuthor(anAuthor);
            return Ok("Author created successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAuthor(string id, [FromBody] Author anAuthor)
        {
            var existingAuthor = await _authorService.GetByID(id);

            if (existingAuthor == null)
            {
                return NotFound();
            }

            await _authorService.UpdateAuthor(anAuthor, id);
            return Ok("Author updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(string id)
        {
            var existingAuthor = await _authorService.GetByID(id);

            if (existingAuthor == null)
            {
                return NotFound();
            }

            await _authorService.RemoveAuthor(id);
            return Ok("Author deleted successfully");
        }

        [HttpGet("{id}/books")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBooksByAuthor(string id)
        {
            var existingAuthor = await _authorService.GetByID(id);

            if (existingAuthor == null)
            {
                return NotFound();
            }

            var books = await _authorService.GetAllBookByAuthor(id);
            return Ok(books);
        }

        [HttpPut("{id}/add-book")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBookToAuthor(string id, [FromBody] string bookId)
        {
            var existingAuthor = await _authorService.GetByID(id);

            if (existingAuthor == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(bookId))
            {
                return BadRequest("Invalid bookId");
            }

            await _authorService.AddBookAuthor(bookId, id);
            return Ok("Added book to the author's list of books");
        }
    }
}
