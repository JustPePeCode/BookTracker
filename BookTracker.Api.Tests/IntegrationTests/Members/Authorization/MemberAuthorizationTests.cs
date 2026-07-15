using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.Authorization;

public class MemberAuthorizationTests : IntegrationTest
{
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
    public async Task MemberCanUpdateOwnAccount()
    {
        var memberId = await AuthenticateAsMember();

        var request = new UpdateMemberRequest
        {
            Name = "Ada Byron",
            Email = "ada.byron@example.com",
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task MemberCannotUpdateAnotherMember()
    {
        var currentMemberId = await AuthenticateAsMember();

        var otherMemberId = SeedMember("Grace Hopper", "grace@example.com");

        var request = new UpdateMemberRequest
        {
            Name = "Changed Name",
            Email = "changed@example.com",
        };

        var response = await Client.PutAsJsonAsync($"/members/{otherMemberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var member = Reader.Query(db => db.Members.Find(otherMemberId));

        Assert.NotNull(member);
        Assert.Equal("Grace Hopper", member.Name.Value);
        Assert.Equal("grace@example.com", member.Email.Value);
    }

    [Fact]
    public async Task MemberCannnotDeleteAnotherMember()
    {
        var currentMemberId = await AuthenticateAsMember();

        var otherMemberId = SeedMember("Grace Hopper", "grace@example.com");

        var response = await Client.DeleteAsync($"/members/{otherMemberId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var member = Reader.Query(db => db.Members.Find(otherMemberId));

        Assert.NotNull(member);
        Assert.Equal("Grace Hopper", member.Name.Value);
        Assert.Equal("grace@example.com", member.Email.Value);
    }

    [Fact]
    public async Task MemberListRequiresAuthentication()
    {
        var response = await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegularMemberCannotViewMemberList()
    {
        await AuthenticateAsMember();

        var response = await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdministratorCanViewMemberList()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MemberRequiresAuthentication()
    {
        var response = await Client.GetAsync("/members/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegularMemberCannotViewMember()
    {
        await AuthenticateAsMember();

        var response = await Client.GetAsync("/members/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdministratorCanViewMember()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/members/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdministratorCanChangeMember()
    {
        var other = new Member
        {
            Name = new MemberName("Jack"),
            Email = new MemberEmail("Jack@Sea.com"),
            PasswordHash = "test-password-hash",
        };
        Writer.Seed(db => db.Members.Add(other));

        await AuthenticateAsMember(MemberRole.Administrator);
        var request = new UpdateMemberRequest
        {
            Name = "Pjotter",
            Email = "Pjotter@MotherRussia.com",
        };

        var response = await Client.PutAsJsonAsync($"/members/{other.Id}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AdministratorCanDeleteMember()
    {
        var other = new Member
        {
            Name = new MemberName("Jack"),
            Email = new MemberEmail("Jack@Sea.com"),
            PasswordHash = "test-password-hash",
        };
        Writer.Seed(db => db.Members.Add(other));

        await AuthenticateAsMember(MemberRole.Administrator);
        var response = await Client.DeleteAsync($"/members/{other.Id}");
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var member = Reader.Query(db => db.Members.Find(other.Id));

        Assert.Null(member);
    }
}
