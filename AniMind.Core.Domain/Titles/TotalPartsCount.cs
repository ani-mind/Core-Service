using Vogen;

namespace AniMind.Core.Domain.Titles;

[ValueObject<int>]
public readonly partial struct TotalPartsCount
{
    private static Validation Validate(int value)
    {
        if (value <= 0)
        {
            return Validation.Invalid("The number of parts listed in the announcement must be greater than 0.");
        }

        return Validation.Ok;
    }
}
