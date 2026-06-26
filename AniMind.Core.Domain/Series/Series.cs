using AniMind.Core.Domain.Shared;
using AniMind.SharedKernel.Core;
using AniMind.SharedKernel.Results;
using Vogen;
using FranchiseId = AniMind.Core.Domain.Franchises.FranchiseId;

namespace AniMind.Core.Domain.Series;

[ValueObject<Guid>]
public readonly partial struct SeriesId
{
    private static Validation Validate(Guid value)
    {
        return value == Guid.Empty
            ? Validation.Invalid($"{nameof(SeriesId)} cannot be empty (Guid.Empty).")
            : Validation.Ok;
    }
}

public sealed class Series : Entity<SeriesId>, IAggregateRoot
{
    public MediaType Type { get; private set; }
    public LocalizedNames Names { get; private set; }
    public BilingualText? Description { get; private set; }
    public FranchiseId? FranchiseId { get; private set; }

    private Series(
        SeriesId id,
        FranchiseId franchiseId,
        MediaType type,
        LocalizedNames names,
        BilingualText? description) : base(id)
    {
        FranchiseId = franchiseId;
        Type = type;
        Names = names;
        Description = description;
    }

    public static Result<Series> Create(
        SeriesId id,
        FranchiseId franchiseId,
        MediaType type,
        LocalizedNames names,
        BilingualText? description) => new Series(id, franchiseId, type, names, description);
}
