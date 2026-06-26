namespace AniMind.SharedKernel.Results;

public readonly record struct Result
{
    private readonly Error? _error;

    private Result(Error? error, bool isSuccess)
    {
        _error = error;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error => !IsSuccess
        ? _error!
        : throw new InvalidOperationException("The error of a success result cannot be accessed.");

    public static Result Success() => new(null, true);

    public static Result Failure(Error error) => new(error, false);

    public static implicit operator Result(Error error) => Failure(error);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);
}
