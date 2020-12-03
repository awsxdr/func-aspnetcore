namespace Func.AspNet
{
    using System;

    public interface IExceptionResponseConverter
    {
        ErrorResponse GetExceptionResponse(Exception exception);
    }
}