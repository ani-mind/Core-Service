using AniMind.Core.Domain.Titles.AiringStates;
using AniMind.Core.Domain.Titles.MediaClassifications;
using AniMind.Core.Domain.Titles.Parts;
using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Titles;

public partial class Title
{
    public static class Errors
    {
        public static Error TooManyParts(TotalPartsCount limit) => Error.Conflict(
            "Title.TooManyParts",
            $"The maximum number of parts for this title has been exceeded. Limit: {limit}.",
            new Dictionary<string, object> { { "totalPartsCount", limit } });

        public static Error ExistingPartsExceedNewLimit(TotalPartsCount limit, int existingCount) =>
            Error.Conflict(
                "Title.ExistingPartsExceedNewLimit",
                $"The existing parts exceed the new limit. New limit: {limit}. Existing count: {existingCount}.",
                new Dictionary<string, object>
                {
                    { "totalPartsCount", limit },
                    { "existingPartsCount", existingCount }
                });

        public static Error CannotFinishAirBeforeAllPartsPremiere() => Error.Conflict(
            "Title.CannotFinishAirBeforeAllPartsPremiere",
            $"Cannot finish airing before the official premiere date and time for all parts.");

        public static Error AgeRatingRequiredForStatus(TitleStatus status) => Error.Validation(
            "Title.AgeRatingRequiredForStatus",
            $"A age rating is required when creating a title with '{status}' status.",
            new Dictionary<string, object> { { "status", status.ToString() } });

        public static Error PartCountRequiredForStatus(TitleStatus status) => Error.Validation(
            "Title.PartCountRequiredForStatus",
            $"A parts count is required for a title with '{status}' status.",
            new Dictionary<string, object> { { "status", status.ToString() } });

        public static Error ReleaseFormatRequiredForStatus(TitleStatus status) => Error.Validation(
            "Title.ReleaseFormatRequiredForStatus",
            $"A release format is required when creating a title with '{status}' status.",
            new Dictionary<string, object> { { "status", status.ToString() } });

        public static Error TitlePremiereAfterPartReleaseTime(
            DateTimeOffset titlePremiere,
            DateTimeOffset partRelease,
            PartNumber partNumber) =>
            Error.Validation(
                "Title.TitlePremiereAfterPartReleaseTime",
                $"The title's official premiere time ({titlePremiere}) cannot be earlier than the part's release time ({partRelease}). Part number: {partNumber}.",
                new Dictionary<string, object>
                {
                    { "titlePremiereTime", titlePremiere },
                    { "partReleaseTime", partRelease },
                    { "partNumber", partNumber }
                });

        public static Error AnnouncedStatusWithNonZeroPartsCount(int partsCount) => Error.Validation(
            "Title.AnnouncedStatusWithNonZeroPartsCount",
            $"The status cannot be 'Announced' for the non zero parts count. The parts count: {partsCount}.",
            new Dictionary<string, object> { { "partsCount", partsCount } });

        public static Error ReleasedTitlePartsCountMismatch(TotalPartsCount totalCount, int actualCount) =>
            Error.Validation(
                "Title.ReleasedTitlePartsCountMismatch",
                $"Cannot transition title to 'Released' status because the number of actual parts ({actualCount}) does not match the announced total parts count ({totalCount.Value}).",
                new Dictionary<string, object>
                {
                    { "totalPartsCount", totalCount.Value },
                    { "actualPartsCount", actualCount }
                });

        public static Error SinglePartFormatCannotHaveMultipleParts(ReleaseFormat releaseFormat) =>
            Error.Validation(
                "Title.SinglePartFormatCannotHaveMultipleParts",
                $"A single part format ({releaseFormat.Name}) cannot have multiple parts",
                new Dictionary<string, object>
                {
                    { "releaseFormat", releaseFormat.Name }
                });

        public static Error SinglePartFormatConflictWithExistingParts(ReleaseFormat format, int existingCount) =>
            Error.Validation(
                "Title.SinglePartFormatConflictWithExistingParts",
                $"Cannot change the release format to '{format.Name}' (a single-part format) because the title already contains {existingCount} parts.",
                new Dictionary<string, object>
                {
                    { "releaseFormat", format.Name },
                    { "existingPartsCount", existingCount }
                });

        public static Error CannotUpdateTotalPartsCountWhenTitleIsReleased() => Error.Conflict(
            "Title.CannotUpdateTotalPartsCountWhenTitleIsReleased",
            $"Cannot update the total parts count when the title is released.");
    }
}
