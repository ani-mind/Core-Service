using System.Reflection;

namespace AniMind.SharedKernel.SmartEnums;

public abstract class SmartEnum<TEnum, TValue> : IEquatable<SmartEnum<TEnum, TValue>>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>
{
    public TValue Value { get; }
    public string Name { get; }

    protected SmartEnum(TValue value, string name)
    {
        Value = value;
        Name = name;
    }

    private static readonly Lazy<Dictionary<TValue, TEnum>> _allItems = new(() =>
        typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f.FieldType == typeof(TEnum))
            .Select(f => (TEnum)f.GetValue(null)!)
            .ToDictionary(e => e.Value));

    public static IReadOnlyCollection<TEnum> GetAll() => _allItems.Value.Values;

    public static TEnum? FromValue(TValue value) => _allItems.Value.GetValueOrDefault(value);

    public bool Equals(SmartEnum<TEnum, TValue>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj) => obj is SmartEnum<TEnum, TValue> other && Equals(other);

    public override int GetHashCode() => EqualityComparer<TValue>.Default.GetHashCode(Value);

    public static bool operator ==(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right) => !(left == right);
}
