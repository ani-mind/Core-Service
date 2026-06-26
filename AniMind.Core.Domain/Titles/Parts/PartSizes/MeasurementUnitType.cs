using AniMind.Core.Domain.Shared;
using AniMind.SharedKernel.SmartEnums;

namespace AniMind.Core.Domain.Titles.Parts.PartSizes;

public abstract class MeasurementUnitType : SmartEnum<MeasurementUnitType, int>
{
    public static readonly MeasurementUnitType Seconds = new SecondsUnit();
    public static readonly MeasurementUnitType Pages = new PagesUnit();
    public static readonly MeasurementUnitType Words = new WordsUnit();
    public static readonly MeasurementUnitType GameplayHours = new GameplayHoursUnit();

    protected MeasurementUnitType(int value, string name) : base(value, name)
    {
    }

    public abstract bool IsSupportedBy(MediaType mediaType);

    private sealed class SecondsUnit() : MeasurementUnitType(1, "Seconds")
    {
        public override bool IsSupportedBy(MediaType mediaType) =>
            mediaType == MediaType.Anime;
    }

    private sealed class PagesUnit() : MeasurementUnitType(2, "Pages")
    {
        public override bool IsSupportedBy(MediaType mediaType) =>
            mediaType == MediaType.Manga || mediaType == MediaType.Manhwa || mediaType == MediaType.Manhua;
    }

    private sealed class WordsUnit() : MeasurementUnitType(3, "Words")
    {
        public override bool IsSupportedBy(MediaType mediaType) =>
            mediaType == MediaType.LightNovel || mediaType == MediaType.Novel;
    }

    private sealed class GameplayHoursUnit() : MeasurementUnitType(4, "GameplayHours")
    {
        public override bool IsSupportedBy(MediaType mediaType) =>
            mediaType == MediaType.VisualNovel;
    }
}
