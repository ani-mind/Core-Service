namespace AniMind.SharedKernel.Results;

public record Error(string Code, string Message, ErrorType Type, IReadOnlyDictionary<string, object>? Metadata = null)
{
    public static readonly Error NullValue = new(
        "Error.NullValue",
        "The specified value is null.",
        ErrorType.Validation);

    public static Error Validation(string code, string message, IReadOnlyDictionary<string, object>? metadata = null) =>
        new(code, message, ErrorType.Validation, metadata);

    public static Error NotFound(string code, string message, IReadOnlyDictionary<string, object>? metadata = null) =>
        new(code, message, ErrorType.NotFound, metadata);

    public static Error Conflict(string code, string message, IReadOnlyDictionary<string, object>? metadata = null) =>
        new(code, message, ErrorType.Conflict, metadata);
}
