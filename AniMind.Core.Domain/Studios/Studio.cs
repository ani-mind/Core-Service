using AniMind.SharedKernel.Core;
using AniMind.SharedKernel.Results;
using Vogen;
using Name = AniMind.Core.Domain.Shared.Name;

namespace AniMind.Core.Domain.Studios;

[ValueObject<Guid>]
public readonly partial struct StudioId
{
    private static Validation Validate(Guid value)
    {
        return value == Guid.Empty
            ? Validation.Invalid($"{nameof(StudioId)} cannot be empty (Guid.Empty).")
            : Validation.Ok;
    }
}

public sealed class Studio : Entity<StudioId>, IAggregateRoot
{
    public Name Name { get; private set; }

    private Studio(StudioId id, Name name) : base(id)
    {
        Name = name;
    }

    public static Result<Studio> Create(StudioId id, Name name) => new Studio(id, name);
}
