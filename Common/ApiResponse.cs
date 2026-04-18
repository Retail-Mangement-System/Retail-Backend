namespace RetailOrdering.API.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;

    // ── Static factory helpers ──────────────────────────────────────────────

    public static ApiResponse<T> SuccessResult(T? data, string message = "")
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> FailResult(string message)
        => new() { Success = false, Data = default, Message = message };

    // Alias used by ExceptionMiddleware
    public static ApiResponse<T> Fail(string message)
        => FailResult(message);

    public static ApiResponse<T> Ok(T? data, string message = "")
      => SuccessResult(data, message);
}