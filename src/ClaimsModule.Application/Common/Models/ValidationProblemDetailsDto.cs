namespace ClaimsModule.Application.Common.Models;

public class ValidationProblemDetailsDto
{
    public string Type { get; set; } = "ValidationError";
    public string Title { get; set; } = "One or more validation errors occurred.";
    public int Status { get; set; } = 422;
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
