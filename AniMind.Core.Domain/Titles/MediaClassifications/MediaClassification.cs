using AniMind.Core.Domain.Shared;
using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Titles.MediaClassifications;

public readonly record struct MediaClassification
{
    public MediaType Type { get; }
    public ReleaseFormat? ReleaseFormat { get; }

    private MediaClassification(MediaType type, ReleaseFormat? releaseFormat)
    {
        Type = type;
        ReleaseFormat = releaseFormat;
    }

    public static Result<MediaClassification> Create(MediaType type, ReleaseFormat? releaseFormat)
    {
        if (releaseFormat != null && !releaseFormat.IsSupportedBy(type))
        {
            return Errors.InvalidFormatForType(type, releaseFormat);
        }

        return new MediaClassification(type, releaseFormat);
    }

    public static class Errors
    {
        public static Error InvalidFormatForType(MediaType type, ReleaseFormat format) => Error.Validation(
            "Title.InvalidFormatForType",
            $"The format '{format}' is not valid for a title of type '{type}'.",
            new Dictionary<string, object>
            {
                { "type", type.Name },
                { "format", format.Name }
            });
    }
}
