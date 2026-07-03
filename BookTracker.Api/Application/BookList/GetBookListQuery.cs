using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<BookInfo>> Execute()
    {
        return await dbContext.Books
        .AsNoTracking()
        .Select(book =>
        new BookInfo
        {
            Id = book.Id,
            Author = book.Author.Value,
            Title = book.Title.Value,
        })
        .ToListAsync();

    }
}