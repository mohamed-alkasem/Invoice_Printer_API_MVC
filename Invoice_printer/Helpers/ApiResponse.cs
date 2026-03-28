namespace Invoice_printer.Helpers
{
    
    public class ApiResponse<T>
    {
        // ── Core properties ──────────────────────────────────────────────────────

        public T? Data { get; set; }

        public string Message { get; set; } = string.Empty;

        public int StatusCode { get; set; }


        public static ApiResponse<T> Ok(T data, string message = "Success") =>
            new() { Data = data, Message = message, StatusCode = 200 };

        public static ApiResponse<T> Created(T data, string message = "Created successfully") =>
            new() { Data = data, Message = message, StatusCode = 201 };

        public static ApiResponse<T> BadRequest(string message = "Bad request") =>
            new() { Data = default, Message = message, StatusCode = 400 };

        public static ApiResponse<T> ValidationFailed(T errors, string message = "Validation failed") =>
            new() { Data = errors, Message = message, StatusCode = 400 };

        public static ApiResponse<T> NotFound(string message = "Resource not found") =>
            new() { Data = default, Message = message, StatusCode = 404 };

        public static ApiResponse<T> ServerError(string message = "An unexpected error occurred") =>
            new() { Data = default, Message = message, StatusCode = 500 };
    }
}
