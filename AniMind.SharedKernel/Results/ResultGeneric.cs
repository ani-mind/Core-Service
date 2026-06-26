namespace AniMind.SharedKernel.Results;

public readonly record struct Result<TValue>
{
    private readonly TValue? _value;
    private readonly Error? _error;

    private Result(TValue? value, Error? error, bool isSuccess)
    {
        _value = value;
        _error = error;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    public Error Error => !IsSuccess
        ? _error!
        : throw new InvalidOperationException("The error of a success result cannot be accessed.");

    public static Result<TValue> Success(TValue value) => new(value, null, true);

    public static Result<TValue> Failure(Error error) => new(default, error, false);

    public static implicit operator Result<TValue>(TValue value) =>
        value is null ? Failure(Error.NullValue) : Success(value);

    public static implicit operator Result<TValue>(Error error) => Failure(error);

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Error);
}
