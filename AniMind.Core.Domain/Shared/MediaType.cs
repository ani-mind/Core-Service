using AniMind.SharedKernel.SmartEnums;

namespace AniMind.Core.Domain.Shared;

public sealed class MediaType : SmartEnum<MediaType, int>
{
    public static readonly MediaType Anime = new(1, "Anime");
    public static readonly MediaType Manga = new(2, "Manga");
    public static readonly MediaType Manhwa = new(3, "Manhwa");
    public static readonly MediaType Manhua = new(4, "Manhua");
    public static readonly MediaType LightNovel = new(5, "LightNovel");
    public static readonly MediaType Novel = new(6, "Novel");
    public static readonly MediaType VisualNovel = new(7, "VisualNovel");

    private MediaType(int value, string name) : base(value, name)
    {
    }
}
