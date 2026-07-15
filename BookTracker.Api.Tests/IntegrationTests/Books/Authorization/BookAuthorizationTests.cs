using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

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

    [Fact]
    public async Task CreateMemberDoesNotRequireAuthentication()
    {
        var request = new CreateMemberRequest
        {
            Name = "Grace Hopper",
            Email = "grace@example.com",
            Password = "debugging-moth",
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateMemberRequiresAuthentication()
    {
        var member = new Member
        {
            Name = new MemberName("Dune"),
            Email = new MemberEmail("Frank@Herbert"),
        };

        Writer.Seed(db => db.Members.Add(member));

        var request = new UpdateMemberRequest
        {
            Name = "Ada Byron",
            Email = "ada.byron@example.com",
        };

        var response = await Client.PutAsJsonAsync($"/members/{member.Id}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegularMemberCannotCreateBook()
    {
        await AuthenticateAsMember();

        var request = new CreateBookRequest
        {
            Title = "Dune",
            Author = "Frank Herbert",
            Year = 1965,
        };

        var response = await Client.PostAsJsonAsync("/books", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var count = Reader.Query(db => db.Books.Count());

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task RegularMemberCannotUpdateBook()
    {
        // Given
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
        await AuthenticateAsMember();

        // When
        var request = new UpdateBookRequest
        {
            Title = new BookTitle("Changed Title"),
            Author = new AuthorName("Changed Author"),
            Year = 2000,
        };
        var response = await Client.PutAsJsonAsync("/books/1", request);
        // Then
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }
}
