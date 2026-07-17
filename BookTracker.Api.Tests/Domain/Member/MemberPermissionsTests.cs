
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Member;

public class MemberPermissionsTests
{
    [Fact]
    public void MemberCanManageOwnAccount()
    {
        var actor =
            new Actor(
                42,
                MemberRole.Member);

        MemberPermissions.EnsureCanManage(
            actor,
            42);
    }

}

