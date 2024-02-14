using System.ComponentModel;
using System.Net;

namespace MG.Dashboard.Api.Services;

public record ServiceResult
{
    protected ServiceResult(HttpStatusCode statusCode, string? message)
    {
        if (Enum.IsDefined(statusCode))
        {
            throw new InvalidEnumArgumentException(nameof(statusCode), (int)statusCode, typeof(HttpStatusCode));
        }

        StatusCode = statusCode;
        Message = message;

        IsSuccess = (int)StatusCode is < 400;
    }

    public bool IsSuccess { get; }

    public HttpStatusCode StatusCode { get; }

    public string? Message { get; }

    public static ServiceResult Success()
    {
        return new ServiceResult(HttpStatusCode.OK, message: null);
    }

    public static ServiceResult Success(HttpStatusCode statusCode)
    {
        return new ServiceResult(statusCode, message: null);
    }

    public static ServiceResult Failure(HttpStatusCode statusCode, string? message = null)
    {
        return new ServiceResult(statusCode, message: message);
    }

    public static ServiceResult<T> Success<T>(T value)
    {
        return new ServiceResult<T>(HttpStatusCode.OK, value, message: null);
    }

    public static ServiceResult<T> Success<T>(HttpStatusCode statusCode, T value)
    {
        return new ServiceResult<T>(statusCode, value, message: null);
    }

    public static ServiceResult<T> Failure<T>(HttpStatusCode statusCode, string? message = null)
    {
        return new ServiceResult<T>(statusCode, value: default, message: message);
    }
}

public sealed record ServiceResult<T> : ServiceResult
{
    internal ServiceResult(HttpStatusCode statusCode, T value, string? message)
        : base(statusCode, message)
    {
        Value = value;
    }

    public T Value { get; }
}