using Vogen;

namespace AniMind.Core.Domain.Shared;

[ValueObject<string>]
public readonly partial struct Name
{
    private static Validation Validate(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Validation.Invalid($"{nameof(Name)} cannot be empty or whitespace.")
            : Validation.Ok;
    }
}
