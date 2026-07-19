namespace GameHub.API.Common.Results;

public class Result
{
    public bool IsSuccess { get; } // opecao realizada

    public bool IsFailure => !IsSuccess; // evita escrever if(!result.IsSuccess)

    public Error? Error { get; } // quando tudo der errado

    protected Result(bool isSuccess, Error? error) // construtor protegido para nao haver new Result
    {
        if (isSuccess && error is not null)
            throw new ArgumentException("Successful result cannot contain an error.");

        if (!isSuccess && error is null)
            throw new ArgumentException("Failed result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }
}
