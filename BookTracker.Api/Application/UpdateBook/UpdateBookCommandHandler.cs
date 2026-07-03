

using BookTracker.Api.Domain;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.UpdateBook;

public class UpdateBookCommandHandler(IBookRepository bookRepository)
{
    public async Task<bool> Execute(int id, UpdateBookRequest request)
    {
        var book =
            new Book
            {
                Id = id,
                Title = new BookTitle(request.Title),// ... create value object here,
                Author = new AuthorName(request.Author),// ... create value object here,
                Year = request.Year
            };


        return await bookRepository.UpdateAsync(book);

    }
}
