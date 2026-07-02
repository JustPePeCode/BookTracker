using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Storage;
using Microsoft.VisualBasic;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> GetAllBooks()
    {
        var books = await bookRepository.GetAllAsync();
        var summary = books.Select(b => new BookInfo
        {
            Id = b.Id,
            Author = b.Author,
            Title = b.Title,
        });
        // Gebruik LINQ om de entiteiten in `books` te mappen naar een `BookInfo` lijst.
        return [.. summary]; // = return summary.ToList()
    }

    public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
    {
        var book = new Book
        {
            Author = request.Author,
            Title = request.Title,
            Year = request.Year,
        };
        await bookRepository.AddAsync(book);
        return new CreateBookResponse
        {
            Id= book.Id,
            Author = book.Author,
            Title = book.Title,
            Year = book.Year,
        };
    }

    public async Task<bool> DeleteBook(int id)
    {
        return await bookRepository.DeleteAsync(id);
    }
}
