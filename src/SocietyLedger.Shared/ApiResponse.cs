namespace SocietyLedger.Shared
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success")
            => new() { Succeeded = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new() { Succeeded = false, Message = message };
    }
}
