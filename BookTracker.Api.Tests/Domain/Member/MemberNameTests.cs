using BookTracker.Api.Domain;


namespace BookTracker.Api.Tests.Domain.Member;

public class MemberNameTests
{
    [Fact]
    public void MemberNameAcceptsValidTitle()
    {
        var name = new MemberName("Mark");

        Assert.Equal("Mark", name.Value);
    }

    [Fact]
    public void BookTitleTrimsValue()
    {
        var name = new MemberName("  Mark  ");

        Assert.Equal("Mark", name.Value);
    }

    [Fact]
    public void BookTitleRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberName("   "));

        Assert.Equal("Name is required.", exception.Message);
    }

    [Fact]
    public void BookTitleRejectsTitleLongerThan100Characters()
    {
        var tooLong = new string('x', 101);

        var exception = Assert.Throws<DomainException>(() => new MemberName(tooLong));

        Assert.Equal("Name cannot be longer than 100 characters.", exception.Message);
    }
}
