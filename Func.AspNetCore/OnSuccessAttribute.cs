namespace Func.AspNetCore
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class OnSuccessAttribute : Attribute
    {
        public int StatusCode { get; }

        public OnSuccessAttribute(int statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
