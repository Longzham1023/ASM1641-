using Microsoft.AspNetCore.Mvc;
using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASM1641_.Controllers
{
    [Route("api/v1/book")]
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BookController(IBookService bookService, IWebHostEnvironment _webHostEnvironment)
        {
            _bookService = bookService;
            this._webHostEnvironment = _webHostEnvironment;
        }

        // GET: api/book
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetBooks([FromQuery] string page)
        {
            var books = await _bookService.GetBooks(int.Parse(page));
            return Ok(books);
        }

        // GET: api/book/5
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetBook(string id)
        {
            var book = await _bookService.GetByID(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        // POST: api/book
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook([FromForm] Book book)
        {
            try
            {
                await _bookService.CreateBook(book, _webHostEnvironment);
                return Ok("Book created successfully!");
            }catch(Exception e)
            {
                return BadRequest($"An error occured: {e}");
            }
            
        }

        // PUT: api/book/5
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(string id, [FromBody] Book book)
        {
            try
            {
                await _bookService.UpdateBook(book, id);
                return Ok("Book updated successfully!");
            }catch(Exception e)
            {
                return BadRequest($"An error occured: {e}");
            }

        }

        // DELETE: api/book/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(string id)
        {
            try
            {
                await _bookService.RemoveBook(id);
                return Ok("Book deleted successfully!");
            }catch(Exception e)
            {
                return BadRequest($"An error occured: {e}");
            }

        }

        // PUT: api/book/addcategory
        [HttpPut("addcategory"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategoryToBook([FromForm] string bookId,[FromForm] string categoryId)
        {
            if (string.IsNullOrEmpty(bookId) || string.IsNullOrEmpty(categoryId))
            {
                return BadRequest("Invalid parameters");
            }

            try
            {
                await _bookService.AddCategoryToBook(bookId, categoryId);
                return Ok("Category added to the book successfully!");
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database errors) and return an error response
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //GET: api/v1/book/getbyname
        [HttpGet("search-by-book-name"), AllowAnonymous]
        public async Task<IActionResult> SearchByName([FromQuery]string page, string bookName)
        {
            try
            {
                var books = await _bookService.SearchBook(bookName, int.Parse(page));
                return Ok(books);
            }
            catch(Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }

        //GET: api/v1/book/getByCategory
        [HttpGet("get-books-by-category/{categoryId}"), AllowAnonymous]
        public async Task<IActionResult> SearchByCategory(string categoryId, [FromQuery]string page)
        {
            try
            {
                var books = await _bookService.GetBookByCategory(categoryId, int.Parse(page));
                return Ok(books);
            }catch(Exception e)
            {
                return BadRequest($"Error: {e}");
            }
        }

    }
}