using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<bool> Execute(int id, UpdateMemberRequest request)
    {
        var member = new Member
        {
            Id = id,
            Name = new MemberName(request.Name), // ... create value object here,
            Email = new MemberEmail(request.Email), // ... create value object here,
        };

        var emailExists = await memberRepository.EmailExistsAsync(member.Email, id);

        if (emailExists)
        {
            throw new MemberEmailAlreadyExistsException();
        }

        return await memberRepository.UpdateAsync(member);
    }
}
