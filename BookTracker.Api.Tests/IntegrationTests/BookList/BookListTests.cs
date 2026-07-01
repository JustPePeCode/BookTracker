using System.Net.Http.Json;
using BookTracker.Api.Application.Booklist;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetBooksReturnsBooks()
    {
        var client = factory.CreateClient();
        var books = await client.GetFromJsonAsync<List<BookInfo>>("/books");
        Assert.NotNull(books);
        Assert.Empty(books);
    }
}
