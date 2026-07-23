using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.GetBookDetails;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Books.UpdateBook;

public class UpdateBookTests : IntegrationTest
{
    [Fact]
    public async Task PutBookUpdatesBook()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965,
                }
            );
        });

        var version = Reader.Query(db =>
            db.Books
                .Where(book => book.Id == 1)
                .Select(book => book.Version)
                .Single());

        var request = new UpdateBookRequest
        {
            Title = "Dune Messiah",
            Author = "Frank Herbert",
            Year = 1969,
            Version = version
        };

        var response = await Client.PutAsJsonAsync("/books/1", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);


        var updatedBook = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(updatedBook);
        Assert.Equal("Dune Messiah", request.Title);
        Assert.Equal("Frank Herbert", request.Author);
        Assert.Equal(1969, request.Year);
        Assert.NotEqual(version, updatedBook.Version);
        // voeg hier de Asserts toe die de properties van book checken
        // gebruik de literal waarden voor de 'expected' values, bvb 1969, niet request.Year
    }

    [Fact]
    public async Task PutBookReturnsNotFoundWhenBookDoesNotExist()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        var request = new UpdateBookRequest
        {
            Title = "Unknown Book",
            Author = "Unknown Author",
            Year = 2000,
        };

        var response = await Client.PutAsJsonAsync("/books/9999", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task PutBookReturnsConflictForStaleVersion()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var firstResponse = await Client.GetAsync("/books/1");

        var firstRead =
            await firstResponse.ReadJsonAs<GetBookDetailsResponse>(
                HttpStatusCode.OK);

        var secondResponse = await Client.GetAsync("/books/1");

        var secondRead =
            await secondResponse.ReadJsonAs<GetBookDetailsResponse>(
                HttpStatusCode.OK);

        var firstRequest =
            new UpdateBookRequest
            {
                Title = "Dune: Special Edition",
                Author = firstRead.Author,
                Year = firstRead.Year,
                Version = firstRead.Version
            };

        var firstUpdateResponse =
            await Client.PutAsJsonAsync("/books/1", firstRequest);

        await firstUpdateResponse.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);

        var secondRequest =
            new UpdateBookRequest
            {
                Title = secondRead.Title,
                Author = secondRead.Author,
                Year = 1966,
                Version = secondRead.Version
            };

        var secondUpdateResponse =
            await Client.PutAsJsonAsync("/books/1", secondRequest);

        await secondUpdateResponse.ShouldHaveStatusCode(
            HttpStatusCode.Conflict);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(book);
        Assert.Equal("Dune: Special Edition", book.Title.Value);
        Assert.Equal(1965, book.Year);
    }
}
