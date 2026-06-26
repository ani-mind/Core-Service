using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Shared;

public sealed record BilingualText
{
    public string Ru { get; }
    public string? En { get; }

    private BilingualText(string ru, string en)
    {
        Ru = ru;
        En = en;
    }

    public static Result<BilingualText> Create(string ru, string en)
    {
        if (string.IsNullOrWhiteSpace(ru))
        {
            return Result<BilingualText>.Failure(Errors.RuEmptyOrWhitespace());
        }

        return new BilingualText(ru.Trim(), en.Trim());
    }

    public static class Errors
    {
        public static Error RuEmptyOrWhitespace() =>
            Error.Validation(
                "BilingualText.RuEmptyOrWhitespace",
                "The russian text you entered is empty or whitespace.");
    }
}
