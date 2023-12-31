﻿using ASM1641_.Dtos;
using ASM1641_.Models;

namespace ASM1641_.IService
{
    public interface IAuthorService
    {
        Task<AuthorResult> GetAllAuthors(int pageNumber);
        Task<Author> GetByID(string id);
        Task CreateAuthor(Author anAuthor);
        Task RemoveAuthor(string Id);
        Task UpdateAuthor(Author anAuthor, string Id);
        Task <IEnumerable<Book>> GetAllBookByAuthor(string authorID);
        Task AddBookAuthor(string bookId, string authorID);
    }
}
