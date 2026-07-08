using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Member;

public class MemberEmailTests
{
    [Fact]
    public void MemberNameAcceptsValidTitle()
    {
        var name = new MemberEmail("Mark@hotmail.com");

        Assert.Equal("Mark@hotmail.com", name.Value);
    }

    [Fact]
    public void BookTitleTrimsValue()
    {
        var name = new MemberEmail("  Mark@hotmail.com  ");

        Assert.Equal("Mark@hotmail.com", name.Value);
    }

    [Fact]
    public void BookTitleRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail("   "));

        Assert.Equal("Email is required.", exception.Message);
    }

    [Fact]
    public void BookTitleRejectsTitleLongerThan100Characters()
    {
        var tooLong = new string('@', 201);

        var exception = Assert.Throws<DomainException>(() => new MemberEmail(tooLong));

        Assert.Equal("Email cannot be longer than 200 characters.", exception.Message);
    }
}
