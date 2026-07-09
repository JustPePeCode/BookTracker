using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.Domain.Books;

public class AuthorNameTests
{
    [Fact]
    public void AuthorNameAcceptsValidName()
    {
        var author = new AuthorName("F. Scott Fitzgerald");

        Assert.Equal("F. Scott Fitzgerald", author.Value);
    }

    [Fact]
    public void AuthorNameTrimsValue()
    {
        var author = new AuthorName("  Mark Meyers   ");
        Assert.Equal("Mark Meyers", author.Value);
        // Implementeer hier deze test
    }

    [Fact]
    public void AuthorNameRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new AuthorName("   "));

        Assert.Equal("Author is required.", exception.Message);
    }

    // Voeg hier de test 'AuthorNameRejectsWhitespace' toe
    // exception.Message = "Author is required."

    [Fact]
    public void AuthorNameRejectsNameLongerThan100Characters()
    {
        // Given
        var exception = Assert.Throws<DomainException>(() =>
            new AuthorName(
                "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901"
            )
        );
        // When

        // Then
        Assert.Equal("Author cannot be longer than 100 characters.", exception.Message);
    }

    [Fact]
    public void AuthorNameRejectsNull()
    {
        var exception = Assert.Throws<DomainException>(() => new AuthorName(null!));

        Assert.Equal("Author is required.", exception.Message);
    }
    // Voeg hier de test 'AuthorNameRejectsNameLongerThan100Characters' toe
    // exception.Message = "Author cannot be longer than 100 characters."
}
