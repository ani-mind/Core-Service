using AniMind.Core.Domain.Shared;
using AniMind.SharedKernel.Core;
using AniMind.SharedKernel.Results;
using Vogen;

namespace AniMind.Core.Domain.Genres;

[ValueObject<Guid>]
public readonly partial struct GenreId
{
    private static Validation Validate(Guid value)
    {
        return value == Guid.Empty
            ? Validation.Invalid($"{nameof(GenreId)} cannot be empty (Guid.Empty).")
            : Validation.Ok;
    }
}

public sealed class Genre : Entity<GenreId>, IAggregateRoot
{
    public BilingualText Name { get; private set; }
    public BilingualText? Description { get; private set; }
    public Genre? OriginGenre { get; private set; }

    private Genre(GenreId id, BilingualText name, BilingualText? description, Genre? originGenre) : base(id)
    {
        OriginGenre = originGenre;
        Name = name;
        Description = description;
    }

    public static Result<Genre>
        Create(GenreId id, BilingualText name, BilingualText? description, Genre? originGenre) =>
        new Genre(id, name, description, originGenre);
}
