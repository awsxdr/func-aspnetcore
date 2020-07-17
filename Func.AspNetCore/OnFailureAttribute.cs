namespace Func.AspNetCore
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class OnFailureAttribute : Attribute
    {
        public int StatusCode { get; }
        public Type ErrorType { get; }
        public string Message { get; set; } = string.Empty;

        internal (int StatusCode, string Message) CodeMessageTuple => (StatusCode, Message);

        public OnFailureAttribute(Type errorType, int statusCode)
        {
            StatusCode = statusCode;
            ErrorType = errorType;
        }
    }
}
