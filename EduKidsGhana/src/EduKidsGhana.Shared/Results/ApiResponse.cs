namespace EduKidsGhana.Shared.Results;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public string? CorrelationId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };

    public static ApiResponse<T> ValidationFail(List<string> errors) =>
        new() { Success = false, Message = "Validation failed", Errors = errors };
}

public class ApiResponse : ApiResponse<object>
{
    public static new ApiResponse<object> Ok(string message = "Success") =>
        new() { Success = true, Message = message };

    public static new ApiResponse<object> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };
}
