namespace GameHub.API.Common.Results;

public sealed record Error(
    string Code,
    string Message); //Um record é adequado para objetos que representam dados e valor.

