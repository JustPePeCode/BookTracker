using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.UpdateBook;
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
        var savedBook = await bookRepository.AddAsync(book);

        return new CreateBookResponse
        {
            Id= savedBook.Id,
            Author = savedBook.Author,
            Title = savedBook.Title,
            Year = savedBook.Year,
        };
    }

    public async Task<bool> DeleteBook(int id)
    {
        return await bookRepository.DeleteAsync(id);
    }
    public async Task<bool> UpdateBook(int id, UpdateBookRequest request)
{
    var book =
        new Book
        {
            Id = id,
            Title = request.Title,
            Author = request.Author,
            Year = request.Year
        };

    return await bookRepository.UpdateAsync(book);
}
}
