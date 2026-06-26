using System.Globalization;
using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Titles.Parts;

public readonly record struct PartNumber : IComparable<PartNumber>
{
    public int Major { get; }

    public int Minor { get; }

    private PartNumber(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }

    public static Result<PartNumber> Create(int major, int minor = 0)
    {
        if (major < 0)
        {
            return Result<PartNumber>.Failure(Errors.NegativeMajor());
        }

        if (minor < 0)
        {
            return Result<PartNumber>.Failure(Errors.NegativeMinor());
        }

        return new PartNumber(major, minor);
    }

    public int CompareTo(PartNumber other)
    {
        var majorComparison = Major.CompareTo(other.Major);
        return majorComparison != 0 ? majorComparison : Minor.CompareTo(other.Minor);
    }

    public override string ToString() => Minor > 0 ? $"{Major}.{Minor}" : Major.ToString(CultureInfo.InvariantCulture);

    public static bool operator <(PartNumber left, PartNumber right) => left.CompareTo(right) < 0;
    public static bool operator >(PartNumber left, PartNumber right) => left.CompareTo(right) > 0;
    public static bool operator <=(PartNumber left, PartNumber right) => left.CompareTo(right) <= 0;
    public static bool operator >=(PartNumber left, PartNumber right) => left.CompareTo(right) >= 0;

    public static class Errors
    {
        public static Error NegativeMajor() =>
            Error.Validation("PartNumber.NegativeMajor", "Major part number cannot be negative.");

        public static Error NegativeMinor() =>
            Error.Validation("PartNumber.NegativeMinor", "Minor part number cannot be negative.");
    }
}
