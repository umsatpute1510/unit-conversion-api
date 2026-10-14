namespace UnitConversion.Utilities.Common.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; init; }
    public bool IsError { get; init; }
    public string? Error { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
}

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(int statusCode, T data, string? message = null) =>
        new() { StatusCode = statusCode, IsError = false, Data = data, Message = message };

    public static ApiResponse<T> Fail<T>(int statusCode, string error) =>
        new() { StatusCode = statusCode, IsError = true, Error = error };
}
