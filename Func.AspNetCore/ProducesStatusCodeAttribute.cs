namespace Func.AspNetCore
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ProducesStatusCodeAttribute : Attribute
    {
        public int StatusCode { get; }
        public string Message { get; } = string.Empty;

        internal (int StatusCode, string Message) CodeMessageTuple => (StatusCode, Message);

        public ProducesStatusCodeAttribute(int statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
