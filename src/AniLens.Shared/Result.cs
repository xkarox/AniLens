namespace AniLens.Shared;

public class Result<T>
{
    private Result(T? data, bool isSuccess, string error = "", int errorType = 0)
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
    public int ErrorType { get; private set; }

    public static Result<T> Success(T data) => new(data, true);
    public static Result<NoData> Success() => new(new NoData(), true);
    public static Result<T> Failure(string error, int errorType = 0) => new(default, false, error, errorType);
}


public class NoData
{
}