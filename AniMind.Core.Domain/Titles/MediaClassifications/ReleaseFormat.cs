using AniMind.Core.Domain.Shared;
using AniMind.SharedKernel.SmartEnums;

namespace AniMind.Core.Domain.Titles.MediaClassifications;

public abstract class ReleaseFormat : SmartEnum<ReleaseFormat, int>
{
    public static readonly ReleaseFormat TvSeries = new AnimeFormat(1, "TvSeries", false);
    public static readonly ReleaseFormat Movie = new AnimeFormat(2, "Movie", true);
    public static readonly ReleaseFormat Ova = new AnimeFormat(3, "Ova", false);
    public static readonly ReleaseFormat Ona = new AnimeFormat(4, "Ona", false);
    public static readonly ReleaseFormat Special = new AnimeFormat(5, "Special", false);

    public static readonly ReleaseFormat OneShot = new ComicFormat(6, "OneShot", true);
    public static readonly ReleaseFormat Serialization = new ComicFormat(7, "Serialization", false);
    public static readonly ReleaseFormat Doujinshi = new ComicFormat(8, "Doujinshi", false);

    public static readonly ReleaseFormat WebNovel = new WrittenFormat(9, "WebNovel", false);

    public static readonly ReleaseFormat Volume = new VolumeFormat(10, "Volume", false);

    protected ReleaseFormat(int value, string name) : base(value, name)
    {
    }

    public abstract bool IsSupportedBy(MediaType mediaType);

    public abstract bool IsSinglePartFormat { get; }

    private sealed class AnimeFormat(int value, string name, bool isSinglePart) : ReleaseFormat(value, name)
    {
        public override bool IsSupportedBy(MediaType mediaType) => mediaType == MediaType.Anime;

        public override bool IsSinglePartFormat => isSinglePart;
    }

    private sealed class ComicFormat(int value, string name, bool isSinglePart) : ReleaseFormat(value, name)
    {
        public override bool IsSupportedBy(MediaType mediaType) =>
            mediaType == MediaType.Manga ||
            mediaType == MediaType.Manhwa ||
            mediaType == MediaType.Manhua;

        public override bool IsSinglePartFormat => isSinglePart;
    }

    private sealed class WrittenFormat(int value, string name, bool isSinglePart) : ReleaseFormat(value, name)
    {
        public override bool IsSupportedBy(MediaType mediaType) =>
            mediaType == MediaType.LightNovel ||
            mediaType == MediaType.Novel;

        public override bool IsSinglePartFormat => isSinglePart;
    }

    private sealed class VolumeFormat(int value, string name, bool isSinglePart) : ReleaseFormat(value, name)
    {
        public override bool IsSupportedBy(MediaType mediaType) =>
            mediaType == MediaType.LightNovel ||
            mediaType == MediaType.Novel ||
            mediaType == MediaType.VisualNovel;

        public override bool IsSinglePartFormat => isSinglePart;
    }
}
