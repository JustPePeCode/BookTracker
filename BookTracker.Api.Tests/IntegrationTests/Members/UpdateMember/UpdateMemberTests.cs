using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Members.UpdateMember;

public class UpdateMemberCommandHandlerMemberTests : IntegrationTest
{
    [Fact]
    public async Task PutMemberReturnsConflictWhenEmailIsAlreadyInUse()
    {
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
                    Name = new MemberName("Jacky"),
                    Email = new MemberEmail("Jacky@Sea.com"),
                    PasswordHash = "test-password-hash",
                }
            );
        });
        var request = new UpdateMemberRequest { Name = "Pjotter", Email = "Jack@Sea.com" };

        var response = await Client.PutAsJsonAsync("/members/2", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
        ;
    }
}
