using System.Net;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

public class GetMemberDetailsQueryHandlerTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberDetailsQueryHandlerReturnsMember()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Jack"),
                    Email = new MemberEmail("Jack@Sea.com"),
                    PasswordHash = "test-password-hash"
                }
            );
        });

        var response = await Client.GetAsync("/members/1");

        var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(member);
        Assert.Equal(1, member.Id);
        Assert.Equal("Jack", member.Name);
        Assert.Equal("jack@sea.com", member.Email);
    }

    [Fact]
    public async Task GetMemberDetailsQueryHandlerReturnsNotFoundWhenMemberDoesNotExist()
    {
        var response = await Client.GetAsync("/members/9999");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
