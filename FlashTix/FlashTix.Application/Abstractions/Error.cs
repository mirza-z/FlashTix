namespace FlashTix.Application.Abstractions;

public enum ErrorType
{
    NotFound,
    Conflict,
    Validation
}

public sealed record Error(string Code, string Description, ErrorType Type)
{
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);
}