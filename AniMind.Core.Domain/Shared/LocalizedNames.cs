using System.Collections.Immutable;
using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Shared;

public sealed record LocalizedNames
{
    /// <summary>
    /// Оригинальное название (например: 進撃の巨人)
    /// </summary>
    public Name Native { get; }

    /// <summary>
    /// Транслитерация (например: Shingeki no Kyojin)
    /// </summary>
    public Name Transliteration { get; }

    /// <summary>
    /// Основной международный перевод (например: Attack on Titan)
    /// </summary>
    public Name? English { get; }

    /// <summary>
    /// Локализованное название для СНГ (например: Атака титанов)
    /// </summary>
    public Name? Russian { get; }

    /// <summary>
    /// Синонимы, аббревиатуры, неофициальные названия (например: AoT, СнК, Shingeki)
    /// </summary>
    public ImmutableArray<Name> Synonyms { get; }

    private LocalizedNames(
        Name native,
        Name transliteration,
        Name? english,
        Name? russian,
        params ReadOnlySpan<Name> synonyms)
    {
        Native = native;
        Transliteration = transliteration;
        English = english;
        Russian = russian;
        Synonyms = [..synonyms];
    }

    public static Result<LocalizedNames> Create(
        Name native,
        Name romaji,
        Name? english,
        Name? russian,
        params ReadOnlySpan<Name> synonyms) =>
        new LocalizedNames(native, romaji, english, russian, synonyms);

    public bool Equals(LocalizedNames? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Native == other.Native &&
               Transliteration == other.Transliteration &&
               English == other.English &&
               Russian == other.Russian &&
               Synonyms.SequenceEqual(other.Synonyms);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Native);
        hash.Add(Transliteration);
        hash.Add(English);
        hash.Add(Russian);

        foreach (var synonym in Synonyms)
        {
            hash.Add(synonym);
        }

        return hash.ToHashCode();
    }
}
