using ASM1641_.IService;
using ASM1641_.Models;
using ASM1641_.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ASM1641_.Dtos;

namespace ASM1641_.Service
{
    public class AuthorService : IAuthorService
    {

        private readonly IMongoCollection<Author> _authorCollection;
        private readonly IMongoCollection<Book> _bookCollection;

        public AuthorService(IOptions<DatabaseSetting> dbSetting)
        {
            var mongoClient = new MongoClient(dbSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSetting.Value.DatabaseName);

            _authorCollection = mongoDatabase.GetCollection<Author>(dbSetting.Value.AuthorCollection);
            _bookCollection = mongoDatabase.GetCollection<Book>(dbSetting.Value.BooksCollection);
        }

        public async Task AddBookAuthor(string bookId, string authorId)
        {
            var authorFilter = Builders<Author>.Filter.Eq("Id", authorId);
            var author = await _authorCollection.FindSync(authorFilter).FirstOrDefaultAsync();

            if (author != null && bookId != null)
            {
                if (author.BookIds == null)
                {
                    author.BookIds = new List<string>();
                }

                if (!author.BookIds.Contains(bookId))
                {
                    author.BookIds.Add(bookId);
                }

                // Update the author document in the Authors collection
                await _authorCollection.ReplaceOneAsync(authorFilter, author);
            }
        }
            public async Task CreateAuthor(Author anAuthor)
        {
            await _authorCollection.InsertOneAsync(anAuthor);
        }


        public async Task RemoveAuthor(string id)
        {
            await _authorCollection.DeleteOneAsync(author => author.Id == id);
        }


        public async Task<AuthorResult> GetAllAuthors(int pageNumber)
        {

            if (pageNumber <= 0)
            {
                throw new ArgumentException("pageNumber must be greater than zero.");
            }

            int skip = (pageNumber - 1) * 10;

            var totalAuthors = await _authorCollection.CountDocumentsAsync(_ => true);
            var totalPages = (int)Math.Ceiling((double)totalAuthors / 10);

            var authors = await _authorCollection.Find(_ => true).Skip(skip).Limit(10).ToListAsync();
              

            return new AuthorResult
            {
                page = pageNumber,
                totalPages = totalPages,
                authors = authors
            };
        }


        public async Task<IEnumerable<Book>> GetAllBookByAuthor(string authorID)
        {
            return await _bookCollection.Find(book => book.AuthorId.Contains(authorID)).ToListAsync();
        }

        public async Task<Author> GetByID(string id)
        {
            return await _authorCollection.Find(author => author.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAuthor(Author anAuthor, string Id)
        {
            var filter = Builders<Author>.Filter.Eq(a => a.Id, anAuthor.Id);
            await _authorCollection.ReplaceOneAsync(filter, anAuthor);
        }

        
    }
}
