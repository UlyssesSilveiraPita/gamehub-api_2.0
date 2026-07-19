namespace GameHub.API.Validation.Models;

public sealed class ValidationErrors
{
    private readonly List<string> _errors = [];

    public bool IsValid => _errors.Count == 0;

    public bool IsInvalid => !IsValid;

    public IReadOnlyCollection<string> Errors => _errors;

    public void Add(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            return;

        _errors.Add(error);
    }
}
