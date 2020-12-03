namespace Func.AspNet
{
    public class DefaultErrorResponseConverter : IErrorResponseConverter
    {
        public ErrorResponse GetErrorResponse<TError>(TError error, ResponseDetails configuredResponseDetails)
            where TError : ResultError 
            =>
                new ErrorResponse
                {
                    StatusCode = configuredResponseDetails.StatusCode,
                    Body = configuredResponseDetails.Message,
                };
    }
}