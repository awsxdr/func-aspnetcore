namespace Func.AspNet
{
    public interface IErrorResponseConverter
    {
        ErrorResponse GetErrorResponse<TError>(TError error, ResponseDetails configuredResponseDetails) where TError : ResultError;
    }
}