using AniMind.Core.Domain.Shared;
using AniMind.Core.Domain.Titles.Parts.PartSizes;
using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Titles.Parts;

public partial class Part
{
    public static class Errors
    {
        public static Error NotFound() =>
            Error.NotFound("Part.NotFound", "The part you entered was not found in this title.");

        public static Error InvalidSequence() =>
            Error.Conflict("Part.InvalidSequence", "The part you entered has an invalid sequence.");

        public static Error DuplicateId(PartId partId) => Error.Conflict(
            "Part.DuplicateId",
            $"A part with id \"{partId}\" for this title already exists.",
            new Dictionary<string, object> { { "partNumber", partId } });

        public static Error DuplicateNumber(PartNumber partNumber) => Error.Conflict(
            "Part.DuplicateNumber",
            $"A part with number \"{partNumber}\" for this title already exists.",
            new Dictionary<string, object> { { "partNumber", partNumber } });

        public static Error CannotAddReleasedPartWithoutTitlePremiere() => Error.Validation(
            "Part.CannotAddReleasedPartWithoutTitlePremiere",
            "Cannot add a part with a specific release time because the title's premiere time is not set.");

        public static Error ReleaseTimeBeforeTitlePremiere(DateTimeOffset titlePremiere, DateTimeOffset partRelease) =>
            Error.Validation(
                "Part.ReleaseTimeBeforeTitlePremiere",
                $"The part's release time ({partRelease}) cannot be earlier than the title's official premiere time ({titlePremiere}).",
                new Dictionary<string, object>
                {
                    { "titlePremiereTime", titlePremiere },
                    { "partReleaseTime", partRelease }
                });

        public static Error InvalidMeasurementUnitForMediaType(MediaType type, MeasurementUnitType unitType) =>
            Error.Validation(
                "Part.InvalidMeasurementUnitForMediaType",
                $"The measurement unit '{unitType}' is not valid for a title of type '{type}'.",
                new Dictionary<string, object>
                {
                    { "type", type.Name },
                    { "measurementUnitType", unitType.Name }
                });
    }
}
