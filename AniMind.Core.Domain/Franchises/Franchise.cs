using AniMind.SharedKernel.Core;
using AniMind.SharedKernel.Results;
using Vogen;
using Name = AniMind.Core.Domain.Shared.Name;

namespace AniMind.Core.Domain.Franchises;

[ValueObject<Guid>]
public readonly partial struct FranchiseId
{
    private static Validation Validate(Guid value)
    {
        return value == Guid.Empty
            ? Validation.Invalid($"{nameof(FranchiseId)} cannot be empty (Guid.Empty).")
            : Validation.Ok;
    }
}

public sealed class Franchise : Entity<FranchiseId>, IAggregateRoot
{
    public Name Name { get; private set; }
    public string? Description { get; private set; }

    private Franchise(FranchiseId id, Name name, string? description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public static Result<Franchise> Create(FranchiseId id, Name name, string? description) =>
        new Franchise(id, name, description);
}
