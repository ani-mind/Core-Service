using AniMind.Core.Domain.Shared;
using AniMind.SharedKernel.Core;
using AniMind.SharedKernel.Results;
using Vogen;

namespace AniMind.Core.Domain.Contributors;

[ValueObject<Guid>]
public readonly partial struct ContributorId
{
    private static Validation Validate(Guid value)
    {
        return value == Guid.Empty
            ? Validation.Invalid($"{nameof(ContributorId)} cannot be empty (Guid.Empty).")
            : Validation.Ok;
    }
}

public sealed class Contributor : Entity<ContributorId>, IAggregateRoot
{
    public BilingualText Name { get; private set; }

    private Contributor(ContributorId id, BilingualText name) : base(id)
    {
        Name = name;
    }

    public static Result<Contributor> Create(ContributorId id, BilingualText name) => new Contributor(id, name);
}
