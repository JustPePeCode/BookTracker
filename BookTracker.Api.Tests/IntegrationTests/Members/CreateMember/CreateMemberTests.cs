using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreateMemberTests : IntegrationTest
{
    [Fact]
    public async Task PostMemberCreatesMember()
    {
        var request = new CreateMemberRequest { Name = "Jack", Email = "Jack@Sea.com" };
        var response = await Client.PostAsJsonAsync("/members", request);

        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Jack", created.Name);

        var member = Reader.Query(context => context.Find<Member>(created.Id));

        Assert.NotNull(member);
        Assert.Equal("Jack", member.Name.Value);
        Assert.Equal("Jack@Sea.com", member.Email.Value);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenTitleIsWhitespace()
    {
        var request = new CreateMemberRequest { Name = "   ", Email = "Jack@Sea.com" };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
