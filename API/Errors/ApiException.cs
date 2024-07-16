namespace API.Errors;

public class ApiException(int statusCode, string message, string? innerDetails)
{
    public int StatusCode { get; set; } = statusCode;

    public string Message { get; set; } = message;

    public string? Details { get; set; } = innerDetails;

}
