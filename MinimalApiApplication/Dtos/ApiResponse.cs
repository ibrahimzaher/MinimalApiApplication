namespace MinimalApiApplication.Dtos;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; }
    public ApiResponse(T data,string message = "Success")
    {
        Success = true;
        Message = message;
        Data = data;
        Timestamp = DateTime.UtcNow;
    }
    public ApiResponse(string message)
    {
        Success = false;
        Message = message;
        Data = default;
        Timestamp = DateTime.UtcNow;
    }

}
