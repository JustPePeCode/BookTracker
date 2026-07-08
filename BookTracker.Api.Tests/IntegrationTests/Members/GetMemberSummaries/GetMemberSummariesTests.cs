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
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Jack"),
                    Email = new MemberEmail("Jack@Sea.com"),
                }
            );
        });

        var response = await Client.GetAsync("/members");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var MemberSummary = Assert.Single(result.Items);

        Assert.Equal("Jack", MemberSummary.Name);
        Assert.Equal("Jack@Sea.com", MemberSummary.Email);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsRequestedPage()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member { Name = new MemberName("Member 1"), Email = new MemberEmail("Name@1") },
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
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Jack"),
                    Email = new MemberEmail("Jack@Sea.com"),
                }
            );
        });

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
        Assert.Equal("Jack@Sea.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByEmail()
    {
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
        Assert.Equal("Woman@Complains.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesAppliesPagingAfterSearch()
    {
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
                },
                new Member
                {
                    Name = new MemberName("Jacky"),
                    Email = new MemberEmail("Jacky@Baby.com"),
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
        var response = await Client.GetAsync("/members?search=dune&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(result.Items);
    }
}
