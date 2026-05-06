namespace Vladify.Frontend.models;

public class ErrorDetails
{
    public required string ErrorTitle { get; set; }
    public required string ErrorMessage { get; set; }
    public required int StatusCode { get; set; }
}
