using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> Execute()
    {
        var books = await bookRepository.GetAllAsync();
        var summary = books.Select(b => new BookInfo
        {
            Id = b.Id,
            Author = b.Author.Value,
            Title = b.Title.Value,
        });
        // Gebruik LINQ om de entiteiten in `books` te mappen naar een `BookInfo` lijst.
        return [.. summary]; // = return summary.ToList()
    }
}