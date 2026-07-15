using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberSummaries;

public class GetMemberSummariesTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberSummariesReturnsMemberSummaries()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/members");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var MemberSummary = Assert.Single(result.Items);

        Assert.Equal("Ada Lovelace", MemberSummary.Name);
        Assert.Equal("ada@example.com", MemberSummary.Email);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsRequestedPage()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member { Name = new MemberName("Member 2"), Email = new MemberEmail("Name@2") },
                new Member { Name = new MemberName("Member 3"), Email = new MemberEmail("Name@3") }
            );
        });

        var result = await Client.GetFromJsonAsync<GetMemberSummariesResponse>(
            "/members?page=2&pageSize=1"
        );

        Assert.NotNull(result);

        var member = Assert.Single(result.Items);

        Assert.Equal("Member 2", member.Name);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsEmptyItemsWhenPageIsTooHigh()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var result = await Client.GetFromJsonAsync<GetMemberSummariesResponse>(
            "/members?page=99&pageSize=10"
        );

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(99, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByName()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Jack"),
                    Email = new MemberEmail("Jack@Sea.com"),
                },
                new Member
                {
                    Name = new MemberName("Woman"),
                    Email = new MemberEmail("Woman@Complains.com"),
                }
            );
        });

        var response = await Client.GetAsync("/members?search=Jack");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Jack", member.Name);
        Assert.Equal("jack@sea.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByEmail()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Jack"),
                    Email = new MemberEmail("Jack@Sea.com"),
                },
                new Member
                {
                    Name = new MemberName("Woman"),
                    Email = new MemberEmail("Woman@Complains.com"),
                }
            );
        });

        var response = await Client.GetAsync("/members?search=Complains");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Woman", member.Name);
        Assert.Equal("woman@complains.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesAppliesPagingAfterSearch()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Jack"),
                    Email = new MemberEmail("Jack@Sea.com"),
                    PasswordHash = "test-password-hash",
                },
                new Member
                {
                    Name = new MemberName("Woman"),
                    Email = new MemberEmail("Woman@Complains.com"),
                    PasswordHash = "test-password-hash",
                },
                new Member
                {
                    Name = new MemberName("Jacky"),
                    Email = new MemberEmail("Jacky@Baby.com"),
                    PasswordHash = "test-password-hash",
                }
            );
        });

        var response = await Client.GetAsync("/members?search=Jack&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Jacky", member.Name);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesListCanBeEmpty()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        var response = await Client.GetAsync("/members?search=dune&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        Assert.Empty(result.Items);
    }
}
