namespace ClaimsModule.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string message)
        : this(new Dictionary<string, string[]> { [propertyName] = [message] })
    {
    }
}
