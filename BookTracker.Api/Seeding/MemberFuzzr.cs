using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;
using QuickFuzzr;

namespace BookTracker.Api.Seeding;

public static class MemberFuzzr
{
    private static readonly PasswordHasher<Member> Hasher = new();

    private static Member CreateMember(string name, string email, string password)
    {
        var member = new Member { Name = new MemberName(name), Email = new MemberEmail(email) };

        member.PasswordHash = Hasher.HashPassword(member, password);

        return member;
    }

    public static IEnumerable<Member> Many(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var members = new List<Member>(count);
        var usedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        while (members.Count < count)
        {
            var member = One.Generate();

            // Replace `.Value` if MemberEmail exposes its value differently.
            if (usedEmails.Add(member.Email.Value))
            {
                members.Add(member);
            }
        }

        return members;
    }

    private static readonly string[] FirstNames =
    [
        "Ada",
        "Grace",
        "Douglas",
        "Ursula",
        "Terry",
        "Octavia",
        "Isaac",
        "Mary",
        "Kurt",
        "Agatha",
        "Ben",
        "Tom",
        "Peter",
        "Lukas",
        "Mark",
        "Sophie",
        "Bob",
    ];

    private static readonly string[] LastNames =
    [
        "Byte",
        "Stackwell",
        "Nullman",
        "Loopington",
        "Brackets",
        "Mergefield",
        "Bugworthy",
        "Semicolon",
        "Heap",
        "Async",
    ];

    private static readonly FuzzrOf<string> Gmail =
        from firstname in Fuzzr.OneOf(FirstNames)
        from lastname in Fuzzr.OneOf(LastNames)
        select $"{firstname}{lastname}@Gmail.com";

    private static readonly FuzzrOf<string> Hotmail =
        from firstname in Fuzzr.OneOf(FirstNames)
        from lastname in Fuzzr.OneOf(LastNames)
        select $"{firstname}{lastname}@Hotmail.com";

    private static readonly FuzzrOf<string> Pswrds =
        from firstname in Fuzzr.OneOf(FirstNames)
        from lastname in Fuzzr.OneOf(LastNames)
        select $"{firstname}{lastname}";

    private static readonly FuzzrOf<string> Email = Fuzzr.OneOf(Gmail, Hotmail);

    private static readonly FuzzrOf<string> Password = Fuzzr.OneOf(Pswrds);
    private static readonly FuzzrOf<string> Name =
        from firstName in Fuzzr.OneOf(FirstNames)
        from lastName in Fuzzr.OneOf(LastNames)
        select $"{firstName}{lastName}";

    private static readonly FuzzrOf<Member> One =
        from name in Name
        from email in Email
        from password in Password
        select CreateMember(name, email, password);
}
