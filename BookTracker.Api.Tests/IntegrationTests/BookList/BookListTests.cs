using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Domain;


namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetBooksReturnsBooks()
    {
        var writer = factory.GetWriter();
        writer.Seed(db => db.Books.Add(
            new Book
            {
                Title = new BookTitle ("Cannery Row"),
                Author = new AuthorName("John Steinbeck"),
                Year = 1945
            }
        ));

        var client = factory.CreateClient();

        var response = await client.GetAsync("/books");
        var books = await response.Content.ReadFromJsonAsync<List<BookInfo>>();

        Assert.NotNull(books);

        var bookInfo = Assert.Single(books);
        Assert.Equal("Cannery Row", bookInfo.Title);
        Assert.Equal("John Steinbeck", bookInfo.Author);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
