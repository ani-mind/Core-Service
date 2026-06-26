using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Titles.Parts.PartSizes;

public readonly record struct PartSize
{
    public MeasurementUnitType MeasurementUnitType { get; }
    public int MeasurementUnitsCount { get; }

    private PartSize(MeasurementUnitType unitType, int unitsCount)
    {
        MeasurementUnitType = unitType;
        MeasurementUnitsCount = unitsCount;
    }

    public static Result<PartSize> Create(MeasurementUnitType unitType, int unitsCount)
    {
        if (unitsCount <= 0)
        {
            return Result<PartSize>.Failure(Errors.NonPositive());
        }

        return new PartSize(unitType, unitsCount);
    }

    public static class Errors
    {
        public static Error NonPositive() =>
            Error.Validation("PartSize.NonPositive", "Part size must be greater than zero.");
    }
}
