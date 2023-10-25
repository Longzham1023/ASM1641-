using System;
using ASM1641_.Data;
using ASM1641_.Dtos;
using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ASM1641_.Service
{
    public class BookService : IBookService
    {
        private readonly IMongoCollection<Book> _bookCollection;
        private readonly IMongoCollection<Author> _authorCollection;
        private readonly IOptions<DatabaseSetting> _dbSettings;

        public BookService(IOptions<DatabaseSetting> dbSetting)
        {
            this._dbSettings = dbSetting;
            var mongoClient = new MongoClient(this._dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(this._dbSettings.Value.DatabaseName);

            _bookCollection = mongoDatabase.GetCollection<Book>(this._dbSettings.Value.BooksCollection);
            _authorCollection = mongoDatabase.GetCollection<Author>(this._dbSettings.Value.AuthorCollection);
        }
        public async Task<BookResult> GetBooks( int pageNumber)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException("pageNumber must be greater than zero.");
            }

            int skip = (pageNumber - 1) * 10;

            var totalBooks = await _bookCollection.CountDocumentsAsync(_ => true);
            var totalPages = (int)Math.Ceiling((double)totalBooks / 10);

            var books = await _bookCollection.Find(_ => true).Skip(skip).Limit(10).ToListAsync();


            return new BookResult
            {
                page = pageNumber,
                totalPages = totalPages,
                books = books
            };
        }
     

        public async Task<Book> GetByID(string id)
        {
            return await _bookCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }




        public async Task CreateBook(Book aBook, IWebHostEnvironment hostingEnvironment)
        {
            if (aBook == null)
            {
                throw new ArgumentNullException(nameof(aBook), "The 'aBook' parameter cannot be null.");
            }

            if (aBook.Image == null)
            {
                throw new ArgumentNullException(nameof(aBook.Image), "The 'Image' property cannot be null.");
            }

            try
            {
                var uploadsPath = Path.Combine(hostingEnvironment.WebRootPath, "images");

                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(aBook.Image.FileName);
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await aBook.Image.CopyToAsync(stream);
                }

                aBook.ImagePath = "/images/" + uniqueFileName;

                await _bookCollection.InsertOneAsync(aBook);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the book: " + ex.Message);
            }
        }



        public async Task UpdateBook(BookDto updatedBook, string bookId, IWebHostEnvironment hostingEnvironment)
        {
            if (updatedBook == null)
            {
                throw new ArgumentNullException(nameof(updatedBook), "The 'updatedBook' parameter cannot be null.");
            }

            var bookFilter = Builders<Book>.Filter.Eq("Id", bookId);
            var book = await _bookCollection.Find(bookFilter).FirstOrDefaultAsync();

            if (book == null)
            {
                throw new Exception("Book not found!");
            }

            if (updatedBook.Image != null)
            {
                // Update the book's image if a new image is provided
                try
                {
                    var uploadsPath = Path.Combine(hostingEnvironment.WebRootPath, "images");

                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(updatedBook.Image.FileName);
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updatedBook.Image.CopyToAsync(stream);
                    }

                    book.ImagePath = "/images/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while updating the book's image: " + ex.Message);
                }
            }

            // Update the book details
            try
            {
                book.Title = updatedBook.Title;
                book.AuthorId = updatedBook.AuthorId;
                book.Description = updatedBook.Description;
                book.Price = updatedBook.Price;
                book.PublishDate = updatedBook.PublishDate;
                book.Publisher = book.Publisher;
                book.BookCategories = updatedBook.BookCategories;
                book.quantity = updatedBook.quantity;

                await _bookCollection.ReplaceOneAsync(bookFilter, book);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the book: " + ex.Message);
            }
        }


        public async Task RemoveBook(string Id)
        {
            await _bookCollection.DeleteOneAsync(e => e.Id == Id);
        }
       
        public async Task AddCategoryToBook(string bookId, string categoryId)
        {
            var filter = Builders<Book>.Filter.Eq("Id", bookId);
            var book = await _bookCollection.Find(filter).FirstOrDefaultAsync();

            if (book != null)
            {
                if (book.BookCategories == null)
                {
                    book.BookCategories = new List<string>();
                }

                if (!book.BookCategories.Contains(categoryId))
                {
                    book.BookCategories.Add(categoryId);
                    await _bookCollection.ReplaceOneAsync(filter, book);
                }
                else
                {
                    throw new Exception($"Already contained category: {categoryId}");
                }
            }
        }
      
        public async Task<BookResult> SearchBook(string bookName, int pageNumber)
        {
            int skip = (pageNumber - 1) * 10;

            var filter = Builders<Book>.Filter.Regex(book => book.Title, new BsonRegularExpression(bookName, "i"));
            var totalBooks = await _bookCollection.CountDocumentsAsync(_ => true);
            var totalPages = (int)Math.Ceiling((double)totalBooks / 10);

            var results = await _bookCollection.Find(filter).Skip(skip).Limit(10).ToListAsync();

            return new BookResult()
            {
                page = pageNumber,
                totalPages = totalPages,
                books = results
            };
        }


        public async Task<BookResult> GetBookByCategory(string aCategory, int pageNumber)
        {
            int pageSize = 10;
            int skip = (pageNumber - 1) * pageSize;

            var filter = Builders<Book>.Filter.AnyEq(book => book.BookCategories, aCategory);

            var totalBooks = await _bookCollection.CountDocumentsAsync(filter);
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            var books = await _bookCollection.Find(filter).Skip(skip).Limit(pageSize).ToListAsync();

            return new BookResult
            {
                page = pageNumber,
                totalPages = totalPages,
                books = books
            };
        }



    }
}

