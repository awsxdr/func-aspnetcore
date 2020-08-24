#if NETFRAMEWORK
namespace Func.AspNetCore
{
    using System;
    using System.Linq;

    internal static class TypeExtensionMethods
    {
        public static TAttribute GetCustomAttribute<TAttribute>(this Type @this) where TAttribute : Attribute =>
            @this.GetCustomAttributes(typeof(TAttribute), false)
            .SingleOrDefault()
            as TAttribute;
    }
}
#endif