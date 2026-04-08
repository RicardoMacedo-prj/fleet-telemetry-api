using FleetTelemetryAPI.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public ErrorType ErrorType { get; }
    public string ErrorMessage { get; }

    private Result(bool isSuccess, T? data, ErrorType errortype, string errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorType = errortype;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T data) =>
        new Result<T>(true, data, ErrorType.None, string.Empty);

    public static Result<T> Failure (ErrorType errorType, string errorMessage) =>
        new Result<T>(false, default, errorType, errorMessage);
}

public class Result
{
    public bool IsSuccess { get; }
    public ErrorType ErrorType { get; }
    public string ErrorMessage { get; }

    private Result(bool isSuccess, ErrorType errorType, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
    }

    public static Result Success() =>
        new Result(true, ErrorType.None, string.Empty);

    public static Result Failure(ErrorType errorType, string errorMessage) =>
        new Result(false, errorType, errorMessage);
}
