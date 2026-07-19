using GameHub.API.Common.Results;

namespace GameHub.API.Extensions;

public static class ResultExtensions
{
    public static TResult Match<TValue, TResult>(
        this Result<TValue> result, //O resultado produzido pelo Service.
        Func<TValue, TResult> onSuccess, //A função executada quando há sucesso.
        Func<Error, TResult> onFailure) //A função executada quando há falha.
    {
        return result.IsSuccess
            ? onSuccess(result.Value!)
            : onFailure(result.Error!);
    }
}
