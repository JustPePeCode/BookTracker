using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreateMemberTests : IntegrationTest
{
    [Fact]
    public async Task PostMemberReturnsBadRequestWhenTitleIsWhitespace()
    {
        var request = new CreateMemberRequest
        {
            Name = "   ",
            Email = "Jack@Sea.com",
            Password = "12345",
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMemberCreatesMember()
    {
        var request = new CreateMemberRequest
        {
            Name = "Jack",
            Email = "Jack@Sea.com",
            Password = "analytical-engine",
        };
        var response = await Client.PostAsJsonAsync("/members", request);

        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Jack", created.Name);

        var member = Reader.Query(db => db.Members.Single(current => current.Id == created.Id));
        var passwordHasher = new PasswordHasher<Member>();

        var result = passwordHasher.VerifyHashedPassword(
            member,
            member.PasswordHash,
            "analytical-engine"
        );

        Assert.NotEqual("analytical-engine", member.PasswordHash);
        Assert.NotNull(member);
        Assert.Equal("Jack", member.Name.Value);
        Assert.Equal("jack@sea.com", member.Email.Value);
        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenPasswordIsWhitespace()
    {
        var request = new CreateMemberRequest
        {
            Name = "Pjotter",
            Email = "Jack@Sea.com",
            Password = "",
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
        ;
    }

    [Fact]
    public async Task PostMemberReturnsConflictWhenEmailIsAlreadyInUse()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Jack"),
                    Email = new MemberEmail("Jack@Sea.com"),
                    PasswordHash = "test-password-hash",
                }
            );
        });
        var request = new CreateMemberRequest
        {
            Name = "Pjotter",
            Email = "Jack@Sea.com",
            Password = "12345678",
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
        ;
    }

    [Fact]
    public async Task CreateMemberCreatesRegularMember()
    {
        var request = new CreateMemberRequest
        {
            Name = "Grace Hopper",
            Email = "grace@example.com",
            Password = "debugging-moth",
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        var member = Reader.Query(db => db.Members.Find(created.Id));

        Assert.NotNull(member);

        Assert.Equal(MemberRole.Member, member.Role);
    }
}
