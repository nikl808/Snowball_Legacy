using System.Diagnostics.CodeAnalysis;

namespace Snowball_Legacy.Server.Utils;

public sealed record ErrorResponse(string Error, ErrorType ErrorType);

public enum ErrorType
{
    None,
    NotFound,
    Invalid,
    Unauthorized,
    InternalServerError
}

public class Result<T>
{
    public T? Value { get; set; }
    public ErrorResponse? Error { get; }
    
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }
    
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<ErrorResponse, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    
    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    private Result(ErrorResponse error)
    {
        Error = error;
        IsSuccess = false;
    }

    private static Result<T> Ok(T value) => new(value);
    private static Result<T> Fail(ErrorType type, string error) => new(new ErrorResponse(error, type));
    public static Result<T> NotFound(string error) => Fail(ErrorType.NotFound, error);
    public static Result<T> Invalid(string error) => Fail(ErrorType.Invalid, error);
    public static Result<T> InternalServerError(string error) => Fail(ErrorType.InternalServerError, error);
    public static implicit operator Result<T>(T value) => Ok(value);
}