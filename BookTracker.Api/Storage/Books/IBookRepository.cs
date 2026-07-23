using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Storage.Books;

public interface IBookRepository
{
    Task<Book> AddAsync(Book book);
    Task<UpdateBookResult> UpdateAsync(Book book, Guid expectedVersion);
    Task<bool> DeleteAsync(int id);
}
