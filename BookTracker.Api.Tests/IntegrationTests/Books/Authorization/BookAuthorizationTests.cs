using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.IntegrationTests.Books.Autherization;

public class BookAuthorizationTests : IntegrationTest
{
    [Fact]
    public async Task CreateBookRequiresAuthentication()
    {
        var request = new CreateBookRequest
        {
            Title = "Dune",
            Author = "Frank Herbert",
            Year = 1965,
        };

        var response = await Client.PostAsJsonAsync("/books", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

        var count = Reader.Query(db => db.Books.Count());

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetBooksDoesNotRequireAuthentication()
    {
        var response = await Client.GetAsync("/books");

        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBooksByIdNotRequireAuthentication()
    {
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

        var response = await Client.GetAsync("/books/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }
}
