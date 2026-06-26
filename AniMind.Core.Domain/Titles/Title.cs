using AniMind.Core.Domain.Series;
using AniMind.Core.Domain.Shared;
using AniMind.Core.Domain.Titles.AiringStates;
using AniMind.Core.Domain.Titles.MediaClassifications;
using AniMind.Core.Domain.Titles.Parts;
using AniMind.Core.Domain.Titles.Parts.PartSizes;
using AniMind.SharedKernel.Core;
using AniMind.SharedKernel.Results;
using Vogen;

namespace AniMind.Core.Domain.Titles;

[ValueObject<Guid>]
public readonly partial struct TitleId
{
    private static Validation Validate(Guid value)
    {
        return value == Guid.Empty
            ? Validation.Invalid($"{nameof(TitleId)} cannot be empty (Guid.Empty).")
            : Validation.Ok;
    }
}

public partial class Title : Entity<TitleId>, IAggregateRoot
{
    public SeriesId SeriesId { get; private set; }
    public LocalizedNames Names { get; private set; }
    public BilingualText? Description { get; private set; }
    public AgeRating? AgeRating { get; private set; }
    public AiringState AiringState { get; private set; }
    public MediaClassification MediaClassification { get; private set; }
    public TotalPartsCount? TotalPartsCount { get; private set; }

    private readonly HashSet<Part> _parts = [];
    public IReadOnlyCollection<Part> Parts => _parts;

    private Title(
        TitleId id,
        SeriesId seriesId,
        LocalizedNames names,
        BilingualText? description,
        AgeRating? ageRating,
        AiringState airingState,
        MediaClassification mediaClassification,
        TotalPartsCount? totalPartsCount)
        : base(id)
    {
        SeriesId = seriesId;
        Names = names;
        Description = description;
        AgeRating = ageRating;
        AiringState = airingState;
        MediaClassification = mediaClassification;
        TotalPartsCount = totalPartsCount;
    }

    public static Result<Title> Create(
        TitleId id,
        SeriesId seriesId,
        LocalizedNames names,
        BilingualText? description,
        AgeRating? ageRating,
        AiringState airingState,
        MediaClassification mediaClassification,
        TotalPartsCount? totalPartsCount)
    {
        var metadataResult = EnsureCanPublishWithStatus(
            airingState.Status,
            ageRating,
            mediaClassification.ReleaseFormat,
            totalPartsCount);
        if (metadataResult.IsFailure)
        {
            return metadataResult.Error;
        }

        if (mediaClassification.ReleaseFormat?.IsSinglePartFormat == true && totalPartsCount > 1)
        {
            return Errors.SinglePartFormatCannotHaveMultipleParts(mediaClassification.ReleaseFormat);
        }

        var title =
            new Title(id, seriesId, names, description, ageRating, airingState, mediaClassification, totalPartsCount);

        return title;
    }

    public Result<PartId> AddPartPlaceholder(PartId partId, PartNumber partNumber)
    {
        var newPartValidationResult = ValidateNewPart(partId, partNumber);
        if (newPartValidationResult.IsFailure)
        {
            return newPartValidationResult.Error;
        }

        var result = Part.CreatePlaceholder(partId, partNumber);
        if (result.IsFailure)
        {
            return Result<PartId>.Failure(result.Error);
        }

        var part = result.Value;
        _parts.Add(part);

        return part.Id;
    }

    public Result<PartId> AddPartWithData(
        PartId partId,
        PartNumber partNumber,
        LocalizedNames name,
        BilingualText? description,
        DateTimeOffset releaseTime,
        PartSize size)
    {
        var newPartValidationResult = ValidateNewPart(partId, partNumber);
        if (newPartValidationResult.IsFailure)
        {
            return newPartValidationResult.Error;
        }

        if (!size.MeasurementUnitType.IsSupportedBy(MediaClassification.Type))
        {
            return Part.Errors.InvalidMeasurementUnitForMediaType(MediaClassification.Type, size.MeasurementUnitType);
        }

        var validateResult = ValidateTitlePremiereTimeForThisPartReleaseTime(releaseTime);
        if (validateResult.IsFailure)
        {
            return validateResult.Error;
        }

        var result = Part.CreateWithData(partId, partNumber, name, description, releaseTime, size);
        if (result.IsFailure)
        {
            return result.Error;
        }

        var part = result.Value;
        _parts.Add(part);

        return part.Id;
    }

    public Result UpdatePart(
        PartId partId,
        LocalizedNames? name,
        BilingualText? description,
        DateTimeOffset? releaseTime,
        PartSize? size)
    {
        var part = _parts.FirstOrDefault(p => p.Id == partId);
        if (part == null)
        {
            return Part.Errors.NotFound();
        }

        if (releaseTime.HasValue)
        {
            var validateResult = ValidateTitlePremiereTimeForThisPartReleaseTime(releaseTime.Value);
            if (validateResult.IsFailure)
            {
                return validateResult.Error;
            }
        }

        if (size.HasValue && size != part.Size &&
            !size.Value.MeasurementUnitType.IsSupportedBy(MediaClassification.Type))
        {
            return Part.Errors.InvalidMeasurementUnitForMediaType(
                MediaClassification.Type,
                size.Value.MeasurementUnitType);
        }

        var result = part.Update(name, description, releaseTime, size);

        return result.IsFailure ? result.Error : Result.Success();
    }

    public Result SetTotalPartsCount(TotalPartsCount newLimit)
    {
        if (TotalPartsCount == newLimit)
        {
            return Result.Success();
        }

        if (AiringState.Status == TitleStatus.Released)
        {
            return Errors.CannotUpdateTotalPartsCountWhenTitleIsReleased();
        }

        if (MediaClassification.ReleaseFormat?.IsSinglePartFormat == true && newLimit > 1)
        {
            return Errors.SinglePartFormatCannotHaveMultipleParts(MediaClassification.ReleaseFormat);
        }

        if (_parts.Count > newLimit.Value)
        {
            return Errors.ExistingPartsExceedNewLimit(newLimit, _parts.Count);
        }

        TotalPartsCount = newLimit;
        return Result.Success();
    }

    public Result ClearTotalPartsCount()
    {
        if (TotalPartsCount == null)
        {
            return Result.Success();
        }

        if (AiringState.Status == TitleStatus.Released)
        {
            return Errors.CannotUpdateTotalPartsCountWhenTitleIsReleased();
        }

        TotalPartsCount = null;
        return Result.Success();
    }

    public Result UpdateMetadata(
        LocalizedNames names,
        BilingualText? description,
        AgeRating? ageRating,
        MediaClassification mediaClassification)
    {
        if (mediaClassification.ReleaseFormat?.IsSinglePartFormat == true && _parts.Count > 1)
        {
            return Errors.SinglePartFormatConflictWithExistingParts(mediaClassification.ReleaseFormat, _parts.Count);
        }

        Names = names;
        Description = description;
        AgeRating = ageRating;
        MediaClassification = mediaClassification;

        return Result.Success();
    }

    public Result UpdateAiringState(AiringState newAiringState, DateTimeOffset currentTimeUtc)
    {
        if (AiringState == newAiringState)
        {
            return Result.Success();
        }

        var invariantsResult = CheckAiringStateInvariants(newAiringState, currentTimeUtc);
        if (invariantsResult.IsFailure)
        {
            return invariantsResult.Error;
        }

        AiringState = newAiringState;

        return Result.Success();
    }

    public Result StartAiring(DateTimeOffset currentTimeUtc)
    {
        var startAiringResult = AiringState.StartAiring(currentTimeUtc);
        return startAiringResult.IsFailure
            ? startAiringResult.Error
            : UpdateAiringState(startAiringResult.Value, currentTimeUtc);
    }

    public Result FinishAiring(DateTimeOffset currentTimeUtc)
    {
        var finishAiringResult = AiringState.FinishAiring(currentTimeUtc);
        return finishAiringResult.IsFailure
            ? finishAiringResult.Error
            : UpdateAiringState(finishAiringResult.Value, currentTimeUtc);
    }

    private static Result EnsureCanPublishWithStatus(
        TitleStatus status,
        AgeRating? ageRating,
        ReleaseFormat? releaseFormat,
        TotalPartsCount? totalPartsCount)
    {
        if (status is not (TitleStatus.Ongoing or TitleStatus.Released))
        {
            return Result.Success();
        }

        if (!ageRating.HasValue)
        {
            return Errors.AgeRatingRequiredForStatus(status);
        }

        if (releaseFormat == null)
        {
            return Errors.ReleaseFormatRequiredForStatus(status);
        }

        if (status is TitleStatus.Released && !totalPartsCount.HasValue)
        {
            return Errors.PartCountRequiredForStatus(status);
        }

        return Result.Success();
    }

    private Result ValidateTitlePremiereTimeForThisPartReleaseTime(DateTimeOffset releaseTime)
    {
        if (!AiringState.PremiereTimeUtc.HasValue)
        {
            return Part.Errors.CannotAddReleasedPartWithoutTitlePremiere();
        }

        if (AiringState.PremiereTimeUtc.Value > releaseTime)
        {
            return Part.Errors.ReleaseTimeBeforeTitlePremiere(AiringState.PremiereTimeUtc.Value, releaseTime);
        }

        return Result.Success();
    }

    private Result ValidateNewPart(PartId partId, PartNumber partNumber)
    {
        if (TotalPartsCount is { } limit && _parts.Count >= limit)
        {
            return Errors.TooManyParts(TotalPartsCount!.Value);
        }

        if (_parts.Any(p => p.Id == partId))
        {
            return Part.Errors.DuplicateId(partId);
        }

        if (_parts.Any(p => p.Number == partNumber))
        {
            return Part.Errors.DuplicateNumber(partNumber);
        }

        var lastPart = _parts.MaxBy(p => p.Number);
        if (lastPart != null && IsSequenceValid(partNumber, lastPart.Number))
        {
            return Part.Errors.InvalidSequence();
        }

        return Result.Success();
    }

    private bool IsSequenceValid(PartNumber current, PartNumber previous)
    {
        if (MediaClassification.Type == MediaType.Anime)
        {
            return current.Major == previous.Major + 1;
        }

        return (current.Minor == previous.Minor + 1 && current.Major == previous.Major) ||
               (current.Major == previous.Major + 1 && current.Minor == 0);
    }

    private Result CheckAiringStateInvariants(AiringState newState, DateTimeOffset currentTimeUtc)
    {
        var metadataResult = ValidateMetadataCompatibility(newState.Status);
        if (metadataResult.IsFailure)
        {
            return metadataResult.Error;
        }

        var partsResult = ValidatePartsCompatibility(newState.Status, currentTimeUtc);
        if (partsResult.IsFailure)
        {
            return partsResult;
        }

        var premiereResult = ValidatePremiereTimeCompatibility(newState.PremiereTimeUtc);
        if (premiereResult.IsFailure)
        {
            return premiereResult;
        }

        return Result.Success();
    }

    private Result ValidateMetadataCompatibility(TitleStatus targetStatus) =>
        EnsureCanPublishWithStatus(targetStatus, AgeRating, MediaClassification.ReleaseFormat, TotalPartsCount);

    private Result ValidatePartsCompatibility(TitleStatus targetStatus, DateTimeOffset currentTimeUtc)
    {
        return targetStatus switch
        {
            TitleStatus.Released when _parts.Any(p => p.ReleaseTime == null || p.ReleaseTime > currentTimeUtc) =>
                Errors.CannotFinishAirBeforeAllPartsPremiere(),

            TitleStatus.Released when !TotalPartsCount.HasValue =>
                Errors.PartCountRequiredForStatus(targetStatus),

            TitleStatus.Released when TotalPartsCount.Value != _parts.Count =>
                Errors.ReleasedTitlePartsCountMismatch(TotalPartsCount.Value, _parts.Count),

            TitleStatus.Announced when _parts.Count != 0 =>
                Errors.AnnouncedStatusWithNonZeroPartsCount(_parts.Count),

            _ => Result.Success()
        };
    }

    private Result ValidatePremiereTimeCompatibility(DateTimeOffset? premiereTimeUtc)
    {
        if (!premiereTimeUtc.HasValue)
        {
            return Result.Success();
        }

        var earliestPart = _parts.MinBy(x => x.ReleaseTime);
        if (earliestPart != null)
        {
            return Result.Failure(Errors.TitlePremiereAfterPartReleaseTime(
                premiereTimeUtc.Value, earliestPart.ReleaseTime!.Value, earliestPart.Number));
        }

        return Result.Success();
    }
}
