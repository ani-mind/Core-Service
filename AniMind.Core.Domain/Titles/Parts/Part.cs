using AniMind.Core.Domain.Shared;
using AniMind.Core.Domain.Titles.Parts.PartSizes;
using AniMind.SharedKernel.Core;
using AniMind.SharedKernel.Results;
using Vogen;

namespace AniMind.Core.Domain.Titles.Parts;

[ValueObject<Guid>]
public readonly partial struct PartId
{
    private static Validation Validate(Guid value)
    {
        return value == Guid.Empty
            ? Validation.Invalid($"{nameof(PartId)} cannot be empty (Guid.Empty).")
            : Validation.Ok;
    }
}

public partial class Part : Entity<PartId>
{
    public PartNumber Number { get; private set; }
    public LocalizedNames? Name { get; private set; }
    public BilingualText? Description { get; private set; }
    public DateTimeOffset? ReleaseTime { get; private set; }
    public PartSize? Size { get; private set; }

    private Part(
        PartId id,
        PartNumber number,
        LocalizedNames? name,
        BilingualText? description,
        DateTimeOffset? releaseTime,
        PartSize? size) : base(id)
    {
        Number = number;
        Name = name;
        Description = description;
        ReleaseTime = releaseTime;
        Size = size;
    }

    internal static Result<Part> CreatePlaceholder(PartId id, PartNumber number) =>
        new Part(id, number, null, null, null, null);

    internal static Result<Part> CreateWithData(
        PartId id,
        PartNumber number,
        LocalizedNames name,
        BilingualText? description,
        DateTimeOffset releaseTime,
        PartSize size) => new Part(id, number, name, description, releaseTime, size);

    internal Result Update(
        LocalizedNames? name,
        BilingualText? description,
        DateTimeOffset? releaseTime,
        PartSize? size)
    {
        Name = name;
        Description = description;
        ReleaseTime = releaseTime;
        Size = size;

        return Result.Success();
    }
}
