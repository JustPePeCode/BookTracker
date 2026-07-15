using System.Net;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

public class GetMemberDetailsQueryHandlerTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberDetailsQueryHandlerReturnsMember()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/members/1");

        var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.NotNull(member);
        Assert.Equal(1, member.Id);
        Assert.Equal("Ada Lovelace", member.Name);
        Assert.Equal("ada@example.com", member.Email);
    }

    [Fact]
    public async Task GetMemberDetailsQueryHandlerReturnsNotFoundWhenMemberDoesNotExist()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        var response = await Client.GetAsync("/members/9999");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}
