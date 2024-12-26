namespace AniLens.Shared;
using static Error;

public class Result<T>
{
    private Result(T? data, bool isSuccess, string error = "", Error errorType = Default)
    {
        Data = data;
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public T? Data { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; private set; }
    public Error ErrorType { get; private set; }

    public static Result<T> Success(T data) => new(data, true);
    public static Result<NoData> Success() => new(new NoData(), true);
    public static Result<T> Failure(string error, Error errorType = Default) => new(default, false, error, errorType);
}


public class NoData
{
}