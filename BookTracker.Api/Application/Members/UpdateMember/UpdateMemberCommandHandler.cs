using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository bookRepository) : IHandler
{
    public async Task<bool> Execute(int id, UpdateMemberRequest request)
    {
        var member = new Member
        {
            Id = id,
            Name = new MemberName(request.Name), // ... create value object here,
            Email = new MemberEmail(request.Email), // ... create value object here,
        };

        return await bookRepository.UpdateAsync(member);
    }
}
