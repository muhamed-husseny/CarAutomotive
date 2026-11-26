namespace CarAutomotive.Core.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;

            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            
            return statusCode switch
            {
               
                400 => "A bad request",
                401 => "You are not authorized",
                403 => "You do not have permission to access this resource",
                404 => "Resource not found",
                500 => "Internal server error",
                _ => null
            };
        }
    }

    public class ApiValidationErrorResponse : ApiResponse
    {
        public ApiValidationErrorResponse() : base(400, "Validation failed")
        {
        }

        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
