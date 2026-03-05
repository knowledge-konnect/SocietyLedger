namespace SocietyLedger.Application.DTOs.User
{
    public class ServiceResult<T>
    {
        public bool Succeeded { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { Succeeded = true, Data = data, StatusCode = 200 };
        public static ServiceResult<T> Failure(string errorCode, string errorMessage, int statusCode) => new ServiceResult<T> { Succeeded = false, ErrorCode = errorCode, ErrorMessage = errorMessage, StatusCode = statusCode };
    }
}