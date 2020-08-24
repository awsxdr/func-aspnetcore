namespace Func.AspNet
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MessageTextSourceAttribute :  Attribute
    {
        public string SourceName { get; }

        public MessageTextSourceAttribute(string sourceName)
        {
            SourceName = sourceName;
        }
    }
}
